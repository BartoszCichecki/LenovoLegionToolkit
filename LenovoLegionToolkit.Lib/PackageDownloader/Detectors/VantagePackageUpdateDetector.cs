using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors;

internal class VantagePackageUpdateDetector
{
    private readonly List<DriverInfo> _driverInfoCache = [];

    public async Task BuildDriverInfoCache()
    {
        var driverInfo = await WMI.Win32.PnpSignedDriver.ReadAsync().ConfigureAwait(false);
        _driverInfoCache.Clear();
        _driverInfoCache.AddRange(driverInfo);
    }

    public async Task<bool> DetectAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var dependenciesSatisfied = await CheckDependenciesSatisfiedAsync(httpClient, document, baseLocation, token).ConfigureAwait(false);
        if (!dependenciesSatisfied)
            return false;

        return await DetectInstallAsync(httpClient, document, baseLocation, token).ConfigureAwait(false);
    }

    private async Task<bool> CheckDependenciesSatisfiedAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var node = document.SelectSingleNode("/Package/Dependencies");
        if (node is null || !node.HasChildNodes)
            return false;

        var rules = CreateRules(node, document, baseLocation);
        if (!AndPackageRule.TryCreate(rules, out var rule))
            return false;

        var result = await rule.CheckDependenciesSatisfiedAsync(_driverInfoCache, httpClient, token).ConfigureAwait(false);
        return result;
    }

    private async Task<bool> DetectInstallAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var node = document.SelectSingleNode("/Package/DetectInstall");
        if (node is null || !node.HasChildNodes)
            return true;

        var rules = CreateRules(node, document, baseLocation);
        if (!AndPackageRule.TryCreate(rules, out var rule))
            return false;

        var result = await rule.DetectInstallNeededAsync(_driverInfoCache, httpClient, token).ConfigureAwait(false);
        return result;
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
                        var rules = CreateRules(childNode, document, baseLocation);
                        if (OrPackageRule.TryCreate(rules, out var value))
                            yield return value;
                        break;
                    }
                case "And":
                    {
                        var rules = CreateRules(childNode, document, baseLocation);
                        if (AndPackageRule.TryCreate(rules, out var value))
                            yield return value;
                        break;
                    }
                case "Not":
                    {
                        var rules = CreateRules(childNode, document, baseLocation);
                        if (NotPackageRule.TryCreate(rules, out var value))
                            yield return value;
                        break;
                    }
                case "_OS":
                    {
                        if (OsPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_WindowsBuildVersion":
                    {
                        if (WindowsBuildVersionPackageRule.TryCreate(childNode, out var value))
                            yield return value;
                        break;
                    }
                case "_CPUAddressWidth":
                    {
                        if (CpuAddressWidthPackageRule.TryCreate(childNode, out var value))
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
                case "_PnPID":
                    {
                        if (PnPIdPackageRule.TryCreate(childNode, out var value))
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
