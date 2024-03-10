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

internal readonly partial struct BiosPackageRule : IPackageRule
{
    [GeneratedRegex("^[A-Z0-9]{4}")]
    private static partial Regex PrefixRegex();

    [GeneratedRegex("[0-9]{2}")]
    private static partial Regex VersionRegex();

    private string[] Levels { get; init; }

    public static bool TryCreate(XmlNode? node, out BiosPackageRule value)
    {
        var levels = node?.SelectNodes("Level")?
            .OfType<XmlNode>()
            .Select(n => n.InnerText)
            .ToArray() ?? [];

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

        var result = Levels.Any((global::System.Func<string, bool>)(level =>
        {
            var levelPrefix = PrefixRegex().Match(level).Value;
            var levelVersion = int.Parse(VersionRegex().Match(level).Value);
            return currentBios.HasValue && levelPrefix == currentBios.Value.Prefix && levelVersion == currentBios.Value.Version;
        }));

        return result;
    }

    public async Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        var currentBios = mi.BiosVersion;

        var result = Levels.All((global::System.Func<string, bool>)(level =>
        {
            var levelPrefix = PrefixRegex().Match(level).Value;
            var levelVersion = int.Parse(VersionRegex().Match(level).Value);
            return currentBios.HasValue && levelPrefix == currentBios.Value.Prefix && levelVersion > currentBios.Value.Version;
        }));

        return result;
    }
}
