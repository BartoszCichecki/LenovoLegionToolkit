using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class PackageDownloader
    {
        private struct PackageDefinition
        {
            public string Location { get; }
            public string Category { get; }

            public PackageDefinition(string location, string category)
            {
                Location = location;
                Category = category;
            }
        }

        private readonly string _catalogBaseUrl = "https://download.lenovo.com/catalog/";

        public async Task<List<Package>> GetPackagesAsync(string machineType, string os, IProgress<float>? progress = null, CancellationToken token = default)
        {
            using var httpClient = new HttpClient();

            progress?.Report(0);

            var packageDefinitions = await GetPackageDefinitionsAsync(httpClient, $"{_catalogBaseUrl}/{machineType}_{os}.xml", token).ConfigureAwait(false);

            var count = 0;
            var totalCount = packageDefinitions.Count;

            var packages = new List<Package>();
            foreach (var packageDefinition in packageDefinitions)
            {
                var package = await GetPackage(httpClient, packageDefinition, token).ConfigureAwait(false);
                packages.Add(package);

                count++;
                progress?.Report((count * 100) / totalCount);
            }

            return packages;
        }

        public async Task<string> DownloadPackageFileAsync(Package package, string location, IProgress<float>? progress = null, CancellationToken token = default)
        {
            using var httpClient = new HttpClient();

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using (var fileStream = File.OpenWrite(tempPath))
                await httpClient.DownloadAsync(package.FileLocation, fileStream, progress, token).ConfigureAwait(false);

            var fileInfo = new FileInfo(tempPath);
            if (fileInfo.Length != package.FileSize)
                throw new InvalidDataException("File size mismatch.");

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

            var finalPath = Path.Combine(location, package.FileName);

            File.Move(tempPath, finalPath, true);

            return finalPath;
        }

        private async Task<List<PackageDefinition>> GetPackageDefinitionsAsync(HttpClient httpClient, string location, CancellationToken token)
        {
            var catalogString = await httpClient.GetStringAsync(location, token).ConfigureAwait(false);

            var document = new XmlDocument();
            document.LoadXml(catalogString);

            var packageNodes = document.SelectNodes("/packages/package");
            if (packageNodes is null)
                return new List<PackageDefinition>();

            var packageDefinitions = new List<PackageDefinition>();
            foreach (var packageNode in packageNodes.OfType<XmlElement>())
            {
                token.ThrowIfCancellationRequested();

                var pLocation = packageNode.SelectSingleNode("location")?.InnerText;
                var pCategory = packageNode.SelectSingleNode("category")?.InnerText;

                if (string.IsNullOrWhiteSpace(pLocation) || string.IsNullOrWhiteSpace(pCategory))
                    continue;

                packageDefinitions.Add(new(pLocation, pCategory));
            }

            return packageDefinitions;
        }

        private async Task<Package> GetPackage(HttpClient httpClient, PackageDefinition packageDefinition, CancellationToken token)
        {
            var location = packageDefinition.Location;
            var baseLocation = location.Remove(location.LastIndexOf("/"));

            var packageString = await httpClient.GetStringAsync(location, token).ConfigureAwait(false);

            var document = new XmlDocument();
            document.LoadXml(packageString);

            var description = document.SelectSingleNode("/Package/Title/Desc")!.InnerText;
            var version = document.SelectSingleNode("/Package/@version")!.InnerText;
            var fileName = document.SelectSingleNode("/Package/Files/Installer/File/Name")!.InnerText;
            var fileSize = int.Parse(document.SelectSingleNode("/Package/Files/Installer/File/Size")!.InnerText);
            var crc = document.SelectSingleNode("/Package/Files/Installer/File/CRC")!.InnerText;
            var releaseDateString = document.SelectSingleNode("/Package/ReleaseDate")!.InnerText;
            var releaseDate = DateTime.Parse(releaseDateString);
            var readMeName = document.SelectSingleNode("/Package/Files/Readme/File/Name")?.InnerText;
            var readMe = await GetReadmeAsync(httpClient, $"{baseLocation}/{readMeName}", token).ConfigureAwait(false);
            var fileLocation = $"{baseLocation}/{fileName}";

            return new(description, version, packageDefinition.Category, fileName, fileSize, crc, releaseDate, readMe, fileLocation);
        }

        private async Task<string?> GetReadmeAsync(HttpClient httpClient, string location, CancellationToken token)
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
    }
}
