using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct PnPIdPackageRule : IPackageRule
{
    private string HardwareId { get; init; }

    public static bool TryCreate(XmlNode? node, out PnPIdPackageRule value)
    {
        var hardwareId = node?.InnerText;

        if (hardwareId is null)
        {
            value = default;
            return false;
        }

        value = new() { HardwareId = hardwareId };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> driverInfoCache, HttpClient _1, CancellationToken _2)
    {
        var result = MatchingDriverInfoExists(HardwareId, driverInfoCache);
        return Task.FromResult(result);
    }

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> driverInfoCache, HttpClient _1, CancellationToken _2)
    {
        var result = MatchingDriverInfoExists(HardwareId, driverInfoCache);
        return Task.FromResult(result);
    }
    private static bool MatchingDriverInfoExists(string hardwareId, IEnumerable<DriverInfo> driverInfoCache)
    {
        return driverInfoCache.Any(di =>
        {
            var result = di.DeviceId.StartsWith(hardwareId, StringComparison.InvariantCultureIgnoreCase);
            result |= di.HardwareId.StartsWith(hardwareId, StringComparison.InvariantCultureIgnoreCase);
            return result;
        });
    }
}
