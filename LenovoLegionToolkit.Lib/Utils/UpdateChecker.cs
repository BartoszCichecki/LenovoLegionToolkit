using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using NeoSmart.AsyncLock;
using Octokit;
using Octokit.Internal;

namespace LenovoLegionToolkit.Lib.Utils;

public class UpdateChecker
{
    private readonly HttpClientFactory _httpClientFactory;
    private readonly UpdateCheckSettings _updateCheckSettings = IoCContainer.Resolve<UpdateCheckSettings>();
    private readonly AsyncLock _updateSemaphore = new();

    private DateTime _lastUpdate;
    private TimeSpan _minimumTimeSpanForRefresh;
    private Update[] _updates = [];

    public bool Disable { get; set; }
    public UpdateCheckStatus Status { get; set; }

    public UpdateChecker(HttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        UpdateMiniumTimeSpanForRefresh();
        _lastUpdate = _updateCheckSettings.Store.LastUpdateCheckDateTime ?? DateTime.MinValue;
    }

    public async Task<Version?> CheckAsync(bool forceCheck)
    {
        using (await _updateSemaphore.LockAsync().ConfigureAwait(false))
        {
            if (Disable)
            {
                _lastUpdate = DateTime.UtcNow;
                _updates = [];
                return null;
            }

            try
            {
                var timeSpanSinceLastUpdate = DateTime.UtcNow - _lastUpdate;
                var shouldCheck = timeSpanSinceLastUpdate > _minimumTimeSpanForRefresh;

                if (!forceCheck && !shouldCheck)
                    return _updates.Length != 0 ? _updates.First().Version : null;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checking...");

                var adapter = new HttpClientAdapter(_httpClientFactory.CreateHandler);
                var productInformation = new ProductHeaderValue("LenovoLegionToolkit-UpdateChecker");
                var connection = new Connection(productInformation, adapter);
                var githubClient = new GitHubClient(connection);
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
                Status = UpdateCheckStatus.Success;

                return _updates.Length != 0 ? _updates.First().Version : null;
            }
            catch (RateLimitExceededException ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Reached API Rate Limitation.", ex);

                Status = UpdateCheckStatus.RateLimitReached;
                return null;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error checking for updates.", ex);

                Status = UpdateCheckStatus.Error;
                return null;
            }
            finally
            {
                _lastUpdate = DateTime.UtcNow;
                _updateCheckSettings.Store.LastUpdateCheckDateTime = _lastUpdate;
                _updateCheckSettings.SynchronizeStore();
            }
        }
    }

    public async Task<Update[]> GetUpdatesAsync()
    {
        using (await _updateSemaphore.LockAsync().ConfigureAwait(false))
            return _updates;
    }

    public async Task<string> DownloadLatestUpdateAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        using (await _updateSemaphore.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            var tempPath = Path.Combine(Folders.Temp, $"LenovoLegionToolkitSetup_{Guid.NewGuid()}.exe");
            var latestUpdate = _updates.OrderByDescending(u => u.Version).FirstOrDefault();

            if (latestUpdate.Equals(default(Update)))
                throw new InvalidOperationException("No updates available");

            if (latestUpdate.Url is null)
                throw new InvalidOperationException("Setup file URL could not be found");

            await using var fileStream = File.OpenWrite(tempPath);
            using var httpClient = _httpClientFactory.Create();
            await httpClient.DownloadAsync(latestUpdate.Url, fileStream, progress, cancellationToken).ConfigureAwait(false);

            return tempPath;
        }
    }

    public void UpdateMiniumTimeSpanForRefresh() => _minimumTimeSpanForRefresh = _updateCheckSettings.Store.UpdateCheckFrequency switch
    {
        UpdateCheckFrequency.PerHour => new(hours: 1, minutes: 0, seconds: 0),
        UpdateCheckFrequency.PerThreeHours => new(hours: 3, minutes: 0, seconds: 0),
        UpdateCheckFrequency.PerTwelveHours => new(hours: 2, minutes: 0, seconds: 0),
        UpdateCheckFrequency.PerDay => new(days: 1, hours: 0, minutes: 0, seconds: 0),
        UpdateCheckFrequency.PerWeek => new(days: 7, hours: 0, minutes: 0, seconds: 0),
        UpdateCheckFrequency.PerMonth => new(days: 30, hours: 0, minutes: 0, seconds: 0),
        _ => throw new ArgumentException(nameof(_updateCheckSettings.Store.UpdateCheckFrequency))
    };
}
