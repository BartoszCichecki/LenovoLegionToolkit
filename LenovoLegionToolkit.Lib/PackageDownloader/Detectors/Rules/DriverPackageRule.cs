using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct DriverPackageRule : IPackageRule
{
    private string[] HardwareIds { get; init; }
    private DateTime? Date { get; init; }
    private Version? Version { get; init; }

    public static bool TryCreate(XmlNode? node, out DriverPackageRule value)
    {
        var hardwareIds = node?.SelectNodes("HardwareID")?
            .OfType<XmlNode>()
            .Select(n => n.InnerText)
            .ToArray() ?? Array.Empty<string>();
        var dateString = node?.SelectSingleNode("Date")?.InnerText;
        var versionString = node?.SelectSingleNode("Version")?.InnerText;

        DateTime? date = null;
        if (DateTime.TryParse(dateString, out var d))
            date = d;

        Version? version = null;
        if (Version.TryParse(RemoveNonVersionCharacters(versionString), out var v))
            version = v;

        if (hardwareIds.IsEmpty())
        {
            value = default;
            return false;
        }

        value = new DriverPackageRule { HardwareIds = hardwareIds, Date = date, Version = version };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> driverInfoCache, HttpClient _1, CancellationToken _2)
    {
        var driverInfo = FindMatchingDriverInfo(HardwareIds, driverInfoCache);

        if (string.IsNullOrEmpty(driverInfo.HardwareId))
            return Task.FromResult(false);

        var result = !VerifyByDateVersion(driverInfo);
        return Task.FromResult(result);
    }

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> driverInfoCache, HttpClient _1, CancellationToken _2)
    {
        var driverInfo = FindMatchingDriverInfo(HardwareIds, driverInfoCache);

        if (string.IsNullOrEmpty(driverInfo.HardwareId))
            return Task.FromResult(false);

        var result = VerifyByDateVersion(driverInfo);
        return Task.FromResult(result);
    }

    private bool VerifyByDateVersion(DriverInfo driverInfo)
    {
        if (Date is not null && driverInfo.Date is not null && Version is not null && driverInfo.Version is not null)
        {
            if (Date < driverInfo.Date)
                return false;

            if (Date == driverInfo.Date)
            {
                var result = Version > driverInfo.Version;
                return result;
            }

            if (Date > driverInfo.Date)
            {
                var result = Version != driverInfo.Version;
                return result;
            }
        }

        if (Version is not null && driverInfo.Version is not null)
        {
            var result = Version > driverInfo.Version;
            return result;
        }

        return false;
    }

    private static string RemoveNonVersionCharacters(string? versionString)
    {
        var arr = versionString?.ToCharArray() ?? Array.Empty<char>();
        arr = Array.FindAll(arr, c => char.IsDigit(c) || c == '.');
        return new string(arr);
    }

    private static DriverInfo FindMatchingDriverInfo(IEnumerable<string> hardwareIds, IEnumerable<DriverInfo> driverInfoCache)
    {
        return driverInfoCache.FirstOrDefault(di => hardwareIds.Any(hardwareId =>
        {
            var result = di.DeviceId.StartsWith(hardwareId, StringComparison.InvariantCultureIgnoreCase);
            result |= di.HardwareId.StartsWith(hardwareId, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }));
    }
}
