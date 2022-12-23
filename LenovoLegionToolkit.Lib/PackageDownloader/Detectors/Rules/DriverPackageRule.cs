using System;
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
    private string? Date { get; init; }
    private string? Version { get; init; }

    public static bool TryCreate(XmlNode? node, out DriverPackageRule value)
    {
        var hardwareIds = node?.SelectNodes("HardwareID")?
            .OfType<XmlNode>()
            .Select(n => n.InnerText)
            .ToArray() ?? Array.Empty<string>();
        var date = node?.SelectSingleNode("Date")?.InnerText;
        var version = node?.SelectSingleNode("Version")?.InnerText;

        if (hardwareIds.IsEmpty())
        {
            value = default;
            return false;
        }

        value = new DriverPackageRule { HardwareIds = hardwareIds, Date = date, Version = version };
        return true;
    }

    public Task<bool> ValidateAsync(HttpClient _1, CancellationToken _2) => Task.FromResult(false);
}