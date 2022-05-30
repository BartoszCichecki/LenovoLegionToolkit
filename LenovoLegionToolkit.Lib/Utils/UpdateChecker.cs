using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using NeoSmart.AsyncLock;
using Octokit;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class UpdateChecker
    {
        private readonly TimeSpan _minimumTimeSpanForRefresh = new(hours: 3, minutes: 0, seconds: 0);
        private readonly AsyncLock _updateSemaphore = new();

        private DateTime _lastUpdate = DateTime.MinValue;
        private Update[] _updates = Array.Empty<Update>();

        internal UpdateChecker() { }

        public async Task<bool> Check()
        {
            using (await _updateSemaphore.LockAsync())
            {
                try
                {
                    var timeSpaneSinceLastUpdate = DateTime.Now - _lastUpdate;
                    var shouldCheck = timeSpaneSinceLastUpdate > _minimumTimeSpanForRefresh;

                    if (!shouldCheck)
                        return _updates.Any();

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Checking...");

                    var githubClient = new GitHubClient(new ProductHeaderValue("LenovoLegionToolkit-UpdateChecker"));
                    var releases = await githubClient.Repository.Release.GetAll("BartoszCichecki", "LenovoLegionToolkit", new ApiOptions { PageSize = 5 }).ConfigureAwait(false);

                    var thisReleaseVersion = Assembly.GetEntryAssembly()?.GetName().Version;

                    var updates = releases
                        .Where(r => !r.Draft)
                        .Select(r => new Update(r))
                        .Where(r => r.Version > thisReleaseVersion)
                        .OrderByDescending(r => r.Version)
                        .ToArray();

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Checked [_updates.Length={updates.Length}]");

                    _updates = updates;

                    return updates.Any();
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Error: {ex.Demystify()}");
                    return false;
                }
                finally
                {
                    _lastUpdate = DateTime.Now;
                }
            }
        }

        public async Task<Update[]> GetUpdates()
        {
            using (await _updateSemaphore.LockAsync())
                return _updates;
        }

        public async Task<string> DownloadLatestUpdate(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            using (await _updateSemaphore.LockAsync(cancellationToken))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"LenovoLegionToolkitSetup_{Guid.NewGuid()}.exe");
                var latestUpdate = _updates.FirstOrDefault();

                if (latestUpdate.Equals(default(Update)))
                    throw new InvalidOperationException("No _updates available");

                if (latestUpdate.Url == null)
                    throw new InvalidOperationException("Setup file URL could not be found");

                using var fileStream = File.OpenWrite(tempPath);
                using var httpClient = new HttpClient();
                await httpClient.DownloadAsync(latestUpdate.Url, fileStream, progress, cancellationToken);

                return tempPath;
            }
        }
    }
}
