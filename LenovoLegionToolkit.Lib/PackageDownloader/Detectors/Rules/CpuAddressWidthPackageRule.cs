using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.System.Management;

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
        var addressWidth = await WMI.Win32.Processor.GetAddressWidthAsync().ConfigureAwait(false);
        var result = Value == addressWidth;
        return result;
    }
}
