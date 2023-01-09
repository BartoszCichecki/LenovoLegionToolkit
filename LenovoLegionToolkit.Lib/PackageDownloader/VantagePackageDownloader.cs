using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.PackageDownloader.Detectors;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class VantagePackageDownloader : AbstractPackageDownloader
{
    private readonly struct PackageDefinition
    {
        public string Location { get; init; }
        public string Category { get; init; }
    }

    private readonly string _catalogBaseUrl = "https://download.lenovo.com/catalog/";

    public override async Task<List<Package>> GetPackagesAsync(string machineType, OS os, IProgress<float>? progress = null, CancellationToken token = default)
    {
        progress?.Report(0);

        var osString = os switch
        {
            OS.Windows11 => "win11",
            OS.Windows10 => "win10",
            OS.Windows8 => "win8",
            OS.Windows7 => "win7",
            _ => throw new ArgumentOutOfRangeException(nameof(os), os, null)
        };

        using var httpClient = new HttpClient();

        var packageDefinitions = await GetPackageDefinitionsAsync(httpClient, $"{_catalogBaseUrl}/{machineType}_{osString}.xml", token).ConfigureAwait(false);

        var updateDetector = new VantagePackageUpdateDetector();
        await updateDetector.BuildDriverInfoCache().ConfigureAwait(false);

        var count = 0;
        var totalCount = packageDefinitions.Count;

        var packages = new List<Package>();
        foreach (var packageDefinition in packageDefinitions)
        {
            var package = await GetPackage(httpClient, updateDetector, packageDefinition, token).ConfigureAwait(false);
            packages.Add(package);

            count++;
            progress?.Report(count * 100 / totalCount);
        }

        return packages;
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

            packageDefinitions.Add(new() { Location = pLocation, Category = pCategory });
        }

        return packageDefinitions;
    }

    private async Task<Package> GetPackage(HttpClient httpClient, VantagePackageUpdateDetector updateDetector, PackageDefinition packageDefinition, CancellationToken token)
    {
        var location = packageDefinition.Location;
        var baseLocation = location.Remove(location.LastIndexOf("/"));

        var packageString = await httpClient.GetStringAsync(location, token).ConfigureAwait(false);

        var document = new XmlDocument();
        document.LoadXml(packageString);

        var id = document.SelectSingleNode("/Package/@id")!.InnerText;
        var title = document.SelectSingleNode("/Package/Title/Desc")!.InnerText;
        var version = document.SelectSingleNode("/Package/@version")!.InnerText;
        var fileName = document.SelectSingleNode("/Package/Files/Installer/File/Name")!.InnerText;
        var fileSizeBytes = int.Parse(document.SelectSingleNode("/Package/Files/Installer/File/Size")!.InnerText);
        var fileSize = $"{fileSizeBytes / 1024.0 / 1024.0:0.00} MB";
        var releaseDateString = document.SelectSingleNode("/Package/ReleaseDate")!.InnerText;
        var releaseDate = DateTime.Parse(releaseDateString);
        var readmeName = document.SelectSingleNode("/Package/Files/Readme/File/Name")?.InnerText;
        var readme = await GetReadmeAsync(httpClient, $"{baseLocation}/{readmeName}", token).ConfigureAwait(false);
        var fileLocation = $"{baseLocation}/{fileName}";

        var isUpdate = false;
        try
        {
            isUpdate = await updateDetector.DetectAsync(httpClient, document, baseLocation, token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't detect update for package {id}. [title={title}, location={location}]", ex);
        }

        return new()
        {
            Id = id,
            Title = title,
            Description = string.Empty,
            Version = version,
            Category = packageDefinition.Category,
            FileName = fileName,
            FileSize = fileSize,
            ReleaseDate = releaseDate,
            Readme = readme,
            FileLocation = fileLocation,
            IsUpdate = isUpdate
        };
    }
}