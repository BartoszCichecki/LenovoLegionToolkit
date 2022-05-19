using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class Update
    {
        public Version Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Url { get; set; }

        public Update(Release release)
        {
            Version = Version.Parse(release.TagName);
            Title = release.Name;
            Description = release.Body;
            Url = release.Assets.Where(ra =>
            {
                var isSetup = ra.Name.Contains("setup", StringComparison.InvariantCultureIgnoreCase);
                var isExe = ra.Name.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase);
                return isSetup && isExe;
            }).Select(ra => ra.BrowserDownloadUrl).FirstOrDefault();
        }
    }

    public class UpdateChecker
    {
        private readonly SemaphoreSlim updateSemaphore = new(1);
        private Update[] updates = Array.Empty<Update>();
        private bool updatesChecked = false;

        public async Task<bool> Check(bool force = false)
        {
            try
            {
                await updateSemaphore.WaitAsync();

                if (!force && updatesChecked)
                    return this.updates.Any();

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
                    Log.Instance.Trace($"Checked [updates.Length={updates.Length}]");

                this.updates = updates;

                return updates.Any();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error: {ex}");
                return false;
            }
            finally
            {
                updatesChecked = true;

                updateSemaphore.Release();
            }
        }

        public async Task<Update[]> GetUpdates()
        {
            try
            {
                await updateSemaphore.WaitAsync();
                return updates;
            }
            finally
            {
                updateSemaphore.Release();
            }
        }

        public async Task<string> DownloadLatestUpdate(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await updateSemaphore.WaitAsync();

                var tempPath = Path.Combine(Path.GetTempPath(), $"LenovoLegionToolkitSetup_{Guid.NewGuid()}.exe");
                var latestUpdate = updates.FirstOrDefault();

                if (latestUpdate == null)
                    throw new InvalidOperationException("No updates available");

                if (latestUpdate.Url == null)
                    throw new InvalidOperationException("Setup file URL could not be found");

                using var fileStream = File.OpenWrite(tempPath);
                using var httpClient = new HttpClient();
                await httpClient.DownloadAsync(latestUpdate.Url, fileStream, progress, cancellationToken);

                return tempPath;
            }
            finally
            {
                updateSemaphore.Release();
            }
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var contentLength = response.Content.Headers.ContentLength;

            using var download = await response.Content.ReadAsStreamAsync(cancellationToken);

            if (progress == null || !contentLength.HasValue)
            {
                await download.CopyToAsync(destination, cancellationToken);
                return;
            }

            var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
            await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
            progress.Report(1);
        }
    }
    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
