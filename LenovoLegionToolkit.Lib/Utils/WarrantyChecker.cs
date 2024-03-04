﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Utils;

public class WarrantyChecker(ApplicationSettings settings, HttpClientFactory httpClientFactory)
{
    public async Task<WarrantyInfo?> GetWarrantyInfo(MachineInformation machineInformation, bool forceRefresh = false, CancellationToken token = default)
    {
        if (!forceRefresh && settings.Store.WarrantyInfo.HasValue)
            return settings.Store.WarrantyInfo.Value;

        using var httpClient = httpClientFactory.Create();

        var warrantyInfo = await GetStandardWarrantyInfo(httpClient, machineInformation, token).ConfigureAwait(false);
        warrantyInfo ??= await GetChineseWarrantyInfo(httpClient, machineInformation, token).ConfigureAwait(false);

        settings.Store.WarrantyInfo = warrantyInfo;
        settings.SynchronizeStore();

        return warrantyInfo;
    }

    private static async Task<WarrantyInfo?> GetStandardWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation, CancellationToken token)
    {
        var content = JsonContent.Create(new { serialNumber = machineInformation.SerialNumber, machineType = machineInformation.MachineType });
        var response = await httpClient.PostAsync("https://pcsupport.lenovo.com/dk/en/api/v4/upsell/redport/getIbaseInfo", content, token).ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        var node = JsonNode.Parse(responseContent);

        if (node is null || node["code"]?.GetValue<int>() != 0)
            return null;

        var baseWarranties = node["data"]?["baseWarranties"]?.AsArray() ?? [];
        var upgradeWarranties = node["data"]?["upgradeWarranties"]?.AsArray() ?? [];

        var startDate = baseWarranties.Concat(upgradeWarranties)
            .Select(n => n?["startDate"])
            .Where(n => n is not null)
            .Select(n => DateTime.Parse(n!.ToString()))
            .Min();
        var endDate = baseWarranties.Concat(upgradeWarranties)
            .Select(n => n?["endDate"])
            .Where(n => n is not null)
            .Select(n => DateTime.Parse(n!.ToString()))
            .Max();

        var productString = await httpClient.GetStringAsync($"https://pcsupport.lenovo.com/dk/en/api/v4/mse/getproducts?productId={machineInformation.SerialNumber}", token).ConfigureAwait(false);
        var productNode = JsonNode.Parse(productString);
        var firstProductNode = (productNode as JsonArray)?.FirstOrDefault();
        var id = firstProductNode?["Id"];
        var link = id is null ? null : new Uri($"https://pcsupport.lenovo.com/products/{id}");

        return new WarrantyInfo(startDate, endDate, link);
    }

    private static async Task<WarrantyInfo?> GetChineseWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation, CancellationToken token)
    {
        var machineInfo = await httpClient
            .GetStringAsync($"https://newsupport.lenovo.com.cn/api/machine/getmachineinfo?sn={machineInformation.SerialNumber}", token)
            .ConfigureAwait(false);

        var node = JsonNode.Parse(machineInfo);
        if (node is null || node["statusCode"]?.GetValue<int>() != 200)
            return null;

        var dataNode = node["data"];
        var startDate = dataNode?["ProductDate"]?.GetValue<DateTime>();

        var driveWarrantyInfo = await httpClient
            .GetStringAsync($"https://newsupport.lenovo.com.cn/api/drive/{machineInformation.SerialNumber}/drivewarrantyinfo", token)
            .ConfigureAwait(false);

        node = JsonNode.Parse(driveWarrantyInfo);
        if (node is null || node["statusCode"]?.GetValue<int>() != 200)
            return null;

        dataNode = node["data"];
        var baseInfoNode = dataNode?["baseinfo"]?.AsArray() ?? [];
        var endDate = baseInfoNode
            .Select(n => n?["EndDate"])
            .Where(n => n is not null)
            .Select(n => DateTime.Parse(n!.ToString()))
            .Max();

        var link = new Uri($"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={machineInformation.SerialNumber}");

        return new WarrantyInfo(startDate, endDate, link);
    }
}
