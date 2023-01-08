using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct OsPackageRule : IPackageRule
{
    private string[] Oses { get; init; }

    public static bool TryCreate(XmlNode? node, out OsPackageRule value)
    {
        var oses = node?.SelectNodes("OS")?
            .OfType<XmlNode>()
            .Select(n => n.InnerText)
            .ToArray() ?? Array.Empty<string>();

        if (oses.IsEmpty())
        {
            value = default;
            return false;
        }

        value = new OsPackageRule { Oses = oses };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => OsVersionMatch();

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => OsVersionMatch();

    private Task<bool> OsVersionMatch()
    {
        var currentOs = OSExtensions.GetCurrent();

        var result = Oses.Any(os =>
        {
            switch (currentOs)
            {
                case OS.Windows11 when os.StartsWith("win11", StringComparison.InvariantCultureIgnoreCase):
                case OS.Windows10 when os.StartsWith("win10", StringComparison.InvariantCultureIgnoreCase):
                case OS.Windows8 when os.StartsWith("win8", StringComparison.InvariantCultureIgnoreCase):
                case OS.Windows7 when os.StartsWith("win7", StringComparison.InvariantCultureIgnoreCase):
                    return true;
                default:
                    return false;
            }
        });

        return Task.FromResult(result);
    }
}
