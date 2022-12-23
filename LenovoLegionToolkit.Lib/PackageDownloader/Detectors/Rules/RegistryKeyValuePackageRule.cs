using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct RegistryKeyValuePackageRule : IPackageRule
{
    private string Key { get; init; }
    private string KeyType { get; init; }
    private string KeyName { get; init; }
    private string Version { get; init; }

    public static bool TryCreate(XmlNode? node, out RegistryKeyValuePackageRule value)
    {
        var key = node?.SelectSingleNode("Key")?.InnerText;
        var keyType = node?.Attributes?.OfType<XmlAttribute>()?.FirstOrDefault(a => a.Name == "type")?.InnerText;
        var keyName = node?.SelectSingleNode("KeyName")?.InnerText;
        var version = node?.SelectSingleNode("Version")?.InnerText;

        if (key is null || keyType is null || keyName is null || version is null)
        {
            value = default;
            return false;
        }

        value = new RegistryKeyValuePackageRule { Key = key, KeyType = keyType, KeyName = keyName, Version = version };
        return true;
    }

    public Task<bool> ValidateAsync(HttpClient _1, CancellationToken _2) => Task.FromResult(false);
}
