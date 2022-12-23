using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

public interface IPackageRule
{
    Task<bool> ValidateAsync(HttpClient httpClient, CancellationToken token);
}
