using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct BiosPackageRule : IPackageRule
{
    private static readonly Regex _prefixRegex = new("^[A-Z0-9]{4}");
    private static readonly Regex _versionRegex = new("[0-9]{2}");

    private string[] Levels { get; init; }

    public static bool TryCreate(XmlNode? node, out BiosPackageRule value)
    {
        var levels = node?.SelectNodes("Level")?
            .OfType<XmlNode>()
            .Select(n => n.InnerText)
            .ToArray() ?? Array.Empty<string>();

        if (levels.IsEmpty())
        {
            value = default;
            return false;
        }

        value = new BiosPackageRule { Levels = levels };
        return true;
    }

    public async Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> driverInfoCache, HttpClient httpClient, CancellationToken token)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        var currentBios = mi.BiosVersion;

        var currentBiosPrefix = _prefixRegex.Match(currentBios).Value;
        var currentBiosVersion = int.Parse(_versionRegex.Match(currentBios).Value);

        var result = Levels.Any(level =>
        {
            var levelPrefix = _prefixRegex.Match(level).Value;
            var levelVersion = int.Parse(_versionRegex.Match(level).Value);
            return levelPrefix == currentBiosPrefix && levelVersion == currentBiosVersion;
        });

        return result;
    }

    public async Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        var currentBios = mi.BiosVersion;

        var currentBiosPrefix = _prefixRegex.Match(currentBios).Value;
        var currentBiosVersion = int.Parse(_versionRegex.Match(currentBios).Value);

        var result = Levels.All(level =>
        {
            var levelPrefix = _prefixRegex.Match(level).Value;
            var levelVersion = int.Parse(_versionRegex.Match(level).Value);
            return levelPrefix == currentBiosPrefix && levelVersion > currentBiosVersion;
        });

        return result;
    }
}
