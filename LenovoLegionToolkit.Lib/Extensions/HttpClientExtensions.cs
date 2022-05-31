using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var contentLength = response.Content.Headers.ContentLength;

            using var download = await response.Content.ReadAsStreamAsync(cancellationToken);

            if (progress is null || !contentLength.HasValue)
            {
                await download.CopyToAsync(destination, cancellationToken);
                return;
            }

            progress.Report(0);
            var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
            await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
            progress.Report(1);
        }
    }
}
