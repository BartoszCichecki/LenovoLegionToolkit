using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct OrPackageRule : IPackageRule
{
    private IEnumerable<IPackageRule> Rules { get; init; }

    public static bool TryCreate(IEnumerable<IPackageRule> rules, out OrPackageRule value)
    {
        value = new OrPackageRule { Rules = rules };
        return true;
    }

    public async Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> driverInfoCache, HttpClient httpClient, CancellationToken token)
    {
        foreach (var rule in Rules)
        {
            if (await rule.CheckDependenciesSatisfiedAsync(driverInfoCache, httpClient, token).ConfigureAwait(false))
                return true;
        }

        return false;
    }

    public async Task<bool> DetectInstallNeededAsync(List<DriverInfo> driverInfoCache, HttpClient httpClient, CancellationToken token)
    {
        foreach (var rule in Rules)
        {
            if (await rule.DetectInstallNeededAsync(driverInfoCache, httpClient, token).ConfigureAwait(false))
                return true;
        }

        return false;
    }
}
