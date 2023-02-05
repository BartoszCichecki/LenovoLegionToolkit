using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using NeoSmart.AsyncLock;
using Octokit;

namespace LenovoLegionToolkit.Lib.Utils;

public class UpdateChecker
{
    private readonly TimeSpan _minimumTimeSpanForRefresh = new(hours: 3, minutes: 0, seconds: 0);
    private readonly AsyncLock _updateSemaphore = new();

    private DateTime _lastUpdate = DateTime.MinValue;
    private Update[] _updates = Array.Empty<Update>();

    public async Task<Version?> Check()
    {
        using (await _updateSemaphore.LockAsync().ConfigureAwait(false))
        {
            try
            {
                var timeSpanSinceLastUpdate = DateTime.UtcNow - _lastUpdate;
                var shouldCheck = timeSpanSinceLastUpdate > _minimumTimeSpanForRefresh;

                if (!shouldCheck)
                    return _updates.Any() ? _updates.First().Version : null;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checking...");

                var githubClient = new GitHubClient(new ProductHeaderValue("LenovoLegionToolkit-UpdateChecker"));
                var releases = await githubClient.Repository.Release.GetAll("BartoszCichecki", "LenovoLegionToolkit", new ApiOptions { PageSize = 5 }).ConfigureAwait(false);

                var thisReleaseVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                var thisBuildDate = Assembly.GetEntryAssembly()?.GetBuildDateTime() ?? new DateTime(2000, 1, 1);

                var updates = releases
                    .Where(r => !r.Draft)
                    .Where(r => !r.Prerelease)
                    .Where(r => (r.PublishedAt ?? r.CreatedAt).UtcDateTime >= thisBuildDate)
                    .Select(r => new Update(r))
                    .Where(r => r.Version > thisReleaseVersion)
                    .OrderByDescending(r => r.Version)
                    .ToArray();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checked [updates.Length={updates.Length}]");

                _updates = updates;

                return _updates.Any() ? _updates.First().Version : null;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error checking for updates.", ex);
                return null;
            }
            finally
            {
                _lastUpdate = DateTime.UtcNow;
            }
        }
    }

    public async Task<Update[]> GetUpdates()
    {
        using (await _updateSemaphore.LockAsync().ConfigureAwait(false))
            return _updates;
    }

    public async Task<string> DownloadLatestUpdate(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        using (await _updateSemaphore.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"LenovoLegionToolkitSetup_{Guid.NewGuid()}.exe");
            var latestUpdate = _updates.OrderByDescending(u => u.Version).FirstOrDefault();

            if (latestUpdate.Equals(default(Update)))
                throw new InvalidOperationException("No updates available");

            if (latestUpdate.Url is null)
                throw new InvalidOperationException("Setup file URL could not be found");

            await using var fileStream = File.OpenWrite(tempPath);
            using var httpClient = new HttpClient();
            await httpClient.DownloadAsync(latestUpdate.Url, fileStream, progress, cancellationToken).ConfigureAwait(false);

            return tempPath;
        }
    }
}