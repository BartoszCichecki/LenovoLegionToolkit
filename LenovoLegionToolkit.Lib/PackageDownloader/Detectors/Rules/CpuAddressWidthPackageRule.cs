using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct CpuAddressWidthPackageRule : IPackageRule
{
    private int Value { get; init; }

    public static bool TryCreate(XmlNode? node, out CpuAddressWidthPackageRule value)
    {
        var addressWidthString = node?.SelectSingleNode("AddressWidth")?.InnerText;

        if (addressWidthString is null || !int.TryParse(addressWidthString, out var addressWidth))
        {
            value = default;
            return false;
        }

        value = new CpuAddressWidthPackageRule { Value = addressWidth };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => CheckCpuAddressWidthAsync();

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => CheckCpuAddressWidthAsync();

    private async Task<bool> CheckCpuAddressWidthAsync()
    {
        var results = await WMI.ReadAsync("root\\CIMV2", $"SELECT * FROM Win32_Processor", pdc =>
        {
            var str = pdc["AddressWidth"].Value.ToString();

            var addressWidth = 0;
            if (int.TryParse(str, out var bn))
                addressWidth = bn;

            return addressWidth;
        }).ConfigureAwait(false);

        var result = Value == results.FirstOrDefault();
        return result;
    }
}
