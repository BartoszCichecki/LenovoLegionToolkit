using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct RegistryKeyPackageRule : IPackageRule
{
    private string Key { get; init; }

    public static bool TryCreate(XmlNode? node, out RegistryKeyPackageRule value)
    {
        var key = node?.SelectSingleNode("Key")?.InnerText;

        if (key is null)
        {
            value = default;
            return false;
        }

        value = new RegistryKeyPackageRule { Key = key };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => KeyExists();

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => KeyExists();

    private Task<bool> KeyExists()
    {
        var hive = Key.Split('\\').FirstOrDefault();
        var path = string.Join('\\', Key.Split('\\').Skip(1));

        if (hive is null || string.IsNullOrEmpty(path))
            return Task.FromResult(false);

        var result = Registry.KeyExists(hive, path);
        return Task.FromResult(result);
    }
}
