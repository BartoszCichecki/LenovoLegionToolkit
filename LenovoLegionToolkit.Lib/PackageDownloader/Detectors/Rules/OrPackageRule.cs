using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct OrPackageRule : IPackageRule
{
    public IEnumerable<IPackageRule> Rules { get; }

    public OrPackageRule(IEnumerable<IPackageRule> rules) => Rules = rules;

    public async Task<bool> ValidateAsync(HttpClient httpClient, CancellationToken token)
    {
        foreach (var rule in Rules)
        {
            if (await rule.ValidateAsync(httpClient, token).ConfigureAwait(false))
                return true;
        }

        return false;
    }
}
