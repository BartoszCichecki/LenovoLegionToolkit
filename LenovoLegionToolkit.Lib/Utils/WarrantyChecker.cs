using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Utils
{
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

        private async Task<WarrantyInfo?> GetStandardWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation,
            CancellationToken token)
        {
            var warrantySummaryString = await httpClient.GetStringAsync($"https://pcsupport.lenovo.com/api/v4/upsellAggregation/vantage/warrantySummaryInfo?geo=us&language=en&sn={machineInformation.SerialNumber}", token).ConfigureAwait(false);

            var warrantySummaryNode = JsonNode.Parse(warrantySummaryString);
            var dataNode = warrantySummaryNode?["data"];

            if (dataNode is null)
                return null;

            var warrantyStatus = dataNode?["warrantyStatus"]?.ToString();
            var startDateString = dataNode?["startDate"]?.ToString();
            var endDateString = dataNode?["endDate"]?.ToString();

            DateTime? startDate = startDateString is null ? null : DateTime.Parse(startDateString);
            DateTime? endDate = endDateString is null ? null : DateTime.Parse(endDateString);

            var productString = await httpClient.GetStringAsync(
                $"https://pcsupport.lenovo.com/dk/en/api/v4/mse/getproducts?productId={machineInformation.SerialNumber}", token).ConfigureAwait(false);

            var productNode = JsonNode.Parse(productString);
            var firstProductNode = (productNode as JsonArray)?.FirstOrDefault();
            var id = firstProductNode?["Id"];

            var link = id is null ? null : new Uri($"https://pcsupport.lenovo.com/products/{id}");

            var warrantyInfo = new WarrantyInfo
            {
                LinkOnly = false,
                Status = warrantyStatus,
                Start = startDate,
                End = endDate,
                Link = link,
            };

            return warrantyInfo;
        }

        private async Task<WarrantyInfo?> GetChineseWarrantyInfo(HttpClient httpClient, MachineInformation machineInformation, CancellationToken token)
        {
            var warrantySummaryString = await httpClient.GetStringAsync($"https://newsupport.lenovo.com.cn/api/drive/{machineInformation.SerialNumber}/drivewarrantyinfo", token).ConfigureAwait(false);

            var warrantySummaryNode = JsonNode.Parse(warrantySummaryString);
            var statusValue = warrantySummaryNode?["statusCode"]?.ToString();

            if (statusValue != "200")
                return null;

            var link = new Uri($"https://newsupport.lenovo.com.cn/deviceGuarantee.html?fromsource=deviceGuarantee&selname={machineInformation.SerialNumber}");

            var warrantyInfo = new WarrantyInfo
            {
                LinkOnly = true,
                Status = null,
                Start = null,
                End = null,
                Link = link,
            };

            return warrantyInfo;
        }
    }
}
