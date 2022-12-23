using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors;

internal class CommercialPackageUpdateDetector
{
    public async Task<bool> DetectAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var detectInstallNode = document.SelectSingleNode("/Package/DetectInstall");
        if (detectInstallNode?.HasChildNodes ?? false)
        {
            var rules = CreateRules(detectInstallNode, document, baseLocation).FirstOrDefault();
            if (rules is null)
                return false;

            var result = await rules.ValidateAsync(httpClient, token).ConfigureAwait(false);
            return result;
        }

        var dependenciesNode = document.SelectSingleNode("/Package/Dependencies");
        if (dependenciesNode?.HasChildNodes ?? false)
        {
            var rules = CreateRules(dependenciesNode, document, baseLocation).FirstOrDefault();
            if (rules is null)
                return false;

            var result = await rules.ValidateAsync(httpClient, token).ConfigureAwait(false);
            return result;
        }

        return false;
    }

    private static IEnumerable<IPackageRule> CreateRules(XmlNode? node, XmlDocument document, string baseLocation)
    {
        if (node is null)
            yield break;

        for (var i = 0; i < node.ChildNodes.Count; i++)
        {
            var childNode = node.ChildNodes[i];
            if (childNode is null)
                continue;

            switch (childNode.Name)
            {
                case "Or":
                    {
                        yield return new OrPackageRule(CreateRules(childNode, document, baseLocation));
                        break;
                    }
                case "And":
                    {
                        yield return new AndPackageRule(CreateRules(childNode, document, baseLocation));
                        break;
                    }
                case "_OS":
                    {
                        if (OsPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_ExternalDetection":
                    {
                        if (ExternalDetectionRule.TryCreate(childNode, document, baseLocation, out var value))
                            yield return value;
                        break;
                    }
                case "_Driver":
                    {
                        if (DriverPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_Bios":
                    {
                        if (BiosPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_RegistryKey":
                    {
                        if (RegistryKeyPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_RegistryKeyValue":
                    {
                        if (RegistryKeyValuePackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                default:
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Unknown rule: {childNode.Name}.");
                        break;
                    }
            }
        }
    }
}
