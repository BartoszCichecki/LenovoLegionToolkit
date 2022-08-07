using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public struct WarrantyInfo
    {
        public string? Status { get; init; }
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }
        public Uri? Link { get; init; }
    }

    public class WarrantyChecker
    {
        private readonly string _warrantySummaryUrl = "https://pcsupport.lenovo.com/api/v4/upsellAggregation/vantage/warrantySummaryInfo?geo=us&language=en&sn=";
        private readonly string _productUrl = "https://pcsupport.lenovo.com/dk/en/api/v4/mse/getproducts?productId=";
        private readonly string _productWebUrl = "https://pcsupport.lenovo.com/products/";

        public async Task<WarrantyInfo> GetWarrantyInfo(MachineInformation machineInformation, CancellationToken token)
        {
            using var httpClient = new HttpClient();

            var warrantySummaryString = await httpClient.GetStringAsync(_warrantySummaryUrl + machineInformation.SerialNumber, token).ConfigureAwait(false);

            var warrantySummaryNode = JsonNode.Parse(warrantySummaryString);
            var dataNode = warrantySummaryNode?["data"];
            var warrantyStatus = dataNode?["warrantyStatus"]?.ToString();
            var startDateString = dataNode?["startDate"]?.ToString();
            var endDateString = dataNode?["endDate"]?.ToString();

            DateTime? startDate = startDateString is null ? null : DateTime.Parse(startDateString);
            DateTime? endDate = endDateString is null ? null : DateTime.Parse(endDateString);

            var productString = await httpClient.GetStringAsync(_productUrl + machineInformation.SerialNumber, token).ConfigureAwait(false);

            var productNode = JsonNode.Parse(productString);
            var firstProductNode = (productNode as JsonArray)?.FirstOrDefault();
            var id = firstProductNode?["Id"];

            var link = id is null ? null : new Uri(_productWebUrl + id);

            return new()
            {
                Status = warrantyStatus,
                Start = startDate,
                End = endDate,
                Link = link,
            };
        }
    }
}
