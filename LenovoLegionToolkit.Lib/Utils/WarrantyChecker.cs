using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Utils;

public class WarrantyChecker
{
    private readonly ApplicationSettings _settings;

    public WarrantyChecker(ApplicationSettings settings)
    {
        _settings = settings;
    }

    public async Task<WarrantyInfo?> GetWarrantyInfo(MachineInformation machineInformation, bool forceRefresh = false, CancellationToken token = default)
    {
        if (!forceRefresh && _settings.Store.WarrantyInfo.HasValue)
            return _settings.Store.WarrantyInfo.Value;

        using var httpClient = new HttpClient();

        var warrantyInfo = await GetStandardWarrantyInfo(httpClient, machineInformation, token);
        warrantyInfo ??= await GetChineseWarrantyInfo(httpClient, machineInformation, token);

        _settings.Store.WarrantyInfo = warrantyInfo;
        _settings.SynchronizeStore();

        return warrantyInfo;
    }

    private static async Task<WarrantyInfo?> GetStandardWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation, CancellationToken token)
    {
        var content = JsonContent.Create(new { serialNumber = machineInformation.SerialNumber, machineType = machineInformation.MachineType });
        var response = await httpClient.PostAsync("https://pcsupport.lenovo.com/dk/en/api/v4/upsell/redport/getIbaseInfo", content, token);
        var responseContent = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        var node = JsonNode.Parse(responseContent);

        var baseWarranties = node?["data"]?["baseWarranties"]?.AsArray() ?? new JsonArray();
        var upgradeWarranties = node?["data"]?["upgradeWarranties"]?.AsArray() ?? new JsonArray();

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

        var warrantyInfo = new WarrantyInfo
        {
            Start = startDate,
            End = endDate,
            Link = link,
        };

        return warrantyInfo;
    }

    private static async Task<WarrantyInfo?> GetChineseWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation, CancellationToken token)
    {
        var warrantySummaryString = await httpClient.GetStringAsync($"https://msupport.lenovo.com.cn/centerapi/devicedetail?sn={machineInformation.SerialNumber}", token).ConfigureAwait(false);

        var warrantySummaryNode = JsonNode.Parse(warrantySummaryString);
        var dataNode = warrantySummaryNode?["data"];

        if (dataNode is null)
            return null;

        var startDateString = dataNode["warranty_start"]?.ToString();
        var endDateString = dataNode["warranty_end"]?.ToString();

        DateTime? startDate = startDateString is null ? null : DateTime.Parse(startDateString);
        DateTime? endDate = endDateString is null ? null : DateTime.Parse(endDateString);

        var link = new Uri($"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={machineInformation.SerialNumber}");

        var warrantyInfo = new WarrantyInfo
        {
            Start = startDate,
            End = endDate,
            Link = link,
        };

        return warrantyInfo;
    }
}