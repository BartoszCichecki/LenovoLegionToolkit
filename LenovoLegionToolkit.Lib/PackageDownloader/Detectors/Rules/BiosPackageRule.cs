using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct BiosPackageRule : IPackageRule
{
    private string Level { get; init; }

    public static bool TryCreate(XmlNode? node, out BiosPackageRule value)
    {
        var level = node?.SelectSingleNode("Level")?.InnerText;

        if (level is null)
        {
            value = default;
            return false;
        }

        value = new BiosPackageRule { Level = level };
        return true;
    }

    public async Task<bool> ValidateAsync(HttpClient _1, CancellationToken _2)
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        var currentBios = mi.BIOSVersion;

        if (!currentBios.Take(4).SequenceEqual(Level.Take(4)))
            return false;

        if (!int.TryParse(currentBios.Skip(4).Take(2).ToArray(), out var currentBiosVersion))
            return false;

        if (!int.TryParse(Level.Skip(4).Take(2).ToArray(), out var levelVersion))
            return false;

        return levelVersion > currentBiosVersion;
    }
}