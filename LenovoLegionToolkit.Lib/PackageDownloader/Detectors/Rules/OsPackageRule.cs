using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct OsPackageRule : IPackageRule
{
    private string Os { get; init; }

    public static bool TryCreate(XmlNode? node, out OsPackageRule value)
    {
        var os = node?.SelectSingleNode("OS")?.InnerText;

        if (os is null)
        {
            value = default;
            return false;
        }

        value = new OsPackageRule { Os = os };
        return true;
    }

    public Task<bool> ValidateAsync(HttpClient _1, CancellationToken _2)
    {
        var result = false;
        var currentOs = OSExtensions.GetCurrent();

        switch (currentOs)
        {
            case OS.Windows11 when Os.StartsWith("win11", StringComparison.InvariantCultureIgnoreCase):
            case OS.Windows10 when Os.StartsWith("win10", StringComparison.InvariantCultureIgnoreCase):
            case OS.Windows8 when Os.StartsWith("win8", StringComparison.InvariantCultureIgnoreCase):
            case OS.Windows7 when Os.StartsWith("win7", StringComparison.InvariantCultureIgnoreCase):
                result = true;
                break;
        }

        return Task.FromResult(result);
    }
}
