using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors;

internal class CommercialPackageUpdateDetector
{
    private List<DriverInfo> _driverInfoCache = new();

    public async Task BuildDriverInfoCache()
    {
        var driverInfo = await WMI.ReadAsync("root\\CIMV2",
            $"SELECT * FROM Win32_PnPSignedDriver",
            pdc =>
            {
                var hardwareId = pdc["HardWareId"].Value as string ?? string.Empty;
                var driverVersionString = pdc["DriverVersion"].Value as string;
                var driverDateString = pdc["DriverDate"].Value as string;

                Version? driverVersion = null;
                if (Version.TryParse(driverVersionString, out var v))
                    driverVersion = v;

                DateTime? driverDate = null;
                if (driverDateString is not null)
                    driverDate = ManagementDateTimeConverter.ToDateTime(driverDateString);

                return new DriverInfo
                {
                    HardwareId = hardwareId,
                    Version = driverVersion,
                    Date = driverDate
                };
            });

        _driverInfoCache.Clear();
        _driverInfoCache.AddRange(driverInfo);
    }

    public async Task<bool> DetectAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var dependenciesSatisfied = await CheckDependenciesSatisfiedAsync(httpClient, document, baseLocation, token).ConfigureAwait(false);
        if (!dependenciesSatisfied)
            return false;

        return await DetectInstallAsync(httpClient, document, baseLocation, token);
    }

    private async Task<bool> CheckDependenciesSatisfiedAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var node = document.SelectSingleNode("/Package/Dependencies");
        if (node is null || !node.HasChildNodes)
            return false;

        var rules = CreateRules(node, document, baseLocation).FirstOrDefault();
        if (rules is null)
            return false;

        var result = await rules.DetectInstallNeededAsync(_driverInfoCache, httpClient, token).ConfigureAwait(false);
        return result;
    }

    private async Task<bool> DetectInstallAsync(HttpClient httpClient, XmlDocument document, string baseLocation, CancellationToken token)
    {
        var node = document.SelectSingleNode("/Package/DetectInstall");
        if (node is null || !node.HasChildNodes)
            return true;

        var rules = CreateRules(node, document, baseLocation).FirstOrDefault();
        if (rules is null)
            return false;

        var result = await rules.DetectInstallNeededAsync(_driverInfoCache, httpClient, token).ConfigureAwait(false);
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
