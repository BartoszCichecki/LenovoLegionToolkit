using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public abstract class AbstractPackageDownloader : IPackageDownloader
{
    public abstract Task<List<Package>> GetPackagesAsync(string machineType, OS os, IProgress<float>? progress = null, CancellationToken token = default);

    public async Task<string> DownloadPackageFileAsync(Package package, string location, IProgress<float>? progress = null, CancellationToken token = default)
    {
        using var httpClient = new HttpClient();

        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        using (var fileStream = File.OpenWrite(tempPath))
            await httpClient.DownloadAsync(package.FileLocation, fileStream, progress, token).ConfigureAwait(false);

        var sha256 = await httpClient.GetStringAsync($"{package.FileLocation}.sha256", token).ConfigureAwait(false);

        using (var fileStream = File.OpenRead(tempPath))
        {
            using var managedSha256 = SHA256.Create();
            var fileSha256Bytes = await managedSha256.ComputeHashAsync(fileStream, token).ConfigureAwait(false);

            var fileSha256 = string.Empty;
            foreach (var b in fileSha256Bytes)
                fileSha256 += b.ToString("x2");

            if (sha256 != fileSha256)
                throw new InvalidDataException("File checksum mismatch.");
        }

        var filename = SanitizeFileName(package.Title) + " - " + package.FileName;
        var finalPath = Path.Combine(location, filename);

        File.Move(tempPath, finalPath, true);

        return finalPath;
    }

    protected async Task<string?> GetReadmeAsync(HttpClient httpClient, string location, CancellationToken token)
    {
        try
        {
            return await httpClient.GetStringAsync(location, token).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    protected static string SanitizeFileName(string name)
    {
        var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        return Regex.Replace(name, invalidRegStr, "_");
    }
}