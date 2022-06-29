using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.Utils
{
    public struct Package
    {
        public string Description { get; }
        public string Category { get; }
        public string FileName { get; }
        public int FileSize { get; }
        public string CRC { get; }
        public DateTime ReleaseDate { get; }
        public string? ReadMe { get; }

        public Package(string description, string category, string fileName, int fileSize, string crc, DateTime releaseDate, string? readMe)
        {
            Description = description;
            Category = category;
            FileName = fileName;
            FileSize = fileSize;
            CRC = crc;
            ReleaseDate = releaseDate;
            ReadMe = readMe;
        }
    }

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
        private readonly string _packagesBaseUrl = "https://download.lenovo.com/pccbbs/mobiles";

        public async Task<List<Package>> GetPackagesAsync(string machineType, string os, CancellationToken token)
        {
            using var httpClient = new HttpClient();

            var packageDefinitions = await GetPackageDefinitionsAsync(httpClient, $"{_catalogBaseUrl}/{machineType}_{os}.xml", token).ConfigureAwait(false);

            var packages = new List<Package>();
            foreach (var packageDefinition in packageDefinitions)
            {
                var package = await GetPackage(httpClient, packageDefinition, token).ConfigureAwait(false);
                packages.Add(package);
            }

            return packages;
        }

        public async Task<string> DownloadPackageFileAsync(Package package, string location, IProgress<float>? progress = null, CancellationToken token = default)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using (var fileStream = File.OpenWrite(tempPath))
            {
                using var httpClient = new HttpClient();
                await httpClient.DownloadAsync($"{_packagesBaseUrl}/{package.FileName}", fileStream, progress, token).ConfigureAwait(false);
            }

            var fileInfo = new FileInfo(tempPath);
            if (fileInfo.Length != package.FileSize)
                throw new InvalidDataException("File size mismatch.");
            
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
            var packageString = await httpClient.GetStringAsync(packageDefinition.Location, token).ConfigureAwait(false);

            var document = new XmlDocument();
            document.LoadXml(packageString);

            var description = document.SelectSingleNode("/Package/Title/Desc")!.InnerText;
            var fileName = document.SelectSingleNode("/Package/Files/Installer/File/Name")!.InnerText;
            var fileSize = int.Parse(document.SelectSingleNode("/Package/Files/Installer/File/Size")!.InnerText);
            var crc = document.SelectSingleNode("/Package/Files/Installer/File/CRC")!.InnerText;
            var releaseDateString = document.SelectSingleNode("/Package/ReleaseDate")!.InnerText;
            var releaseDate = DateTime.Parse(releaseDateString);
            var readMeName = document.SelectSingleNode("/Package/Files/Readme/File/Name")?.InnerText;
            var readMe = await GetReadmeAsync(httpClient, $"{_packagesBaseUrl}/{readMeName}", token).ConfigureAwait(false);

            return new(description, packageDefinition.Category, fileName, fileSize, crc, releaseDate, readMe);
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
