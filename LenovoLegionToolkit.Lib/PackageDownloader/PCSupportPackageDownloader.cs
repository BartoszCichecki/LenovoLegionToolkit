using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class PCSupportPackageDownloader(HttpClientFactory httpClientFactory)
    : AbstractPackageDownloader(httpClientFactory)
{
    private const string CATALOG_BASE_URL = "https://pcsupport.lenovo.com/us/en/api/v4/downloads/drivers?productId=";

    public override async Task<List<Package>> GetPackagesAsync(string machineType, OS os, IProgress<float>? progress = null, CancellationToken token = default)
    {
        var osString = os switch
        {
            OS.Windows11 => "Windows 11",
            OS.Windows10 => "Windows 10",
            OS.Windows8 => "Windows 8",
            OS.Windows7 => "Windows 7",
            _ => throw new InvalidOperationException(nameof(os)),
        };

        using var httpClient = HttpClientFactory.Create();
        httpClient.DefaultRequestHeaders.Referrer = new Uri("https://pcsupport.lenovo.com/");

        progress?.Report(0);

        var catalogJson = await httpClient.GetStringAsync($"{CATALOG_BASE_URL}{machineType}", token).ConfigureAwait(false);
        var catalogJsonNode = JsonNode.Parse(catalogJson);
        var downloadsNode = catalogJsonNode?["body"]?["DownloadItems"]?.AsArray();

        if (downloadsNode is null)
            return [];

        var packages = new List<Package>();
        foreach (var downloadNode in downloadsNode)
        {
            if (!IsCompatible(downloadNode, osString))
                continue;

            var package = ParsePackage(downloadNode!);
            if (package is null)
                continue;

            packages.Add(package.Value);
        }

        return packages;
    }

    private static Package? ParsePackage(JsonNode downloadNode)
    {
        var id = downloadNode["ID"]!.ToJsonString();
        var category = downloadNode["Category"]!["Name"]!.ToString();
        var title = downloadNode["Title"]!.ToString();
        var description = downloadNode["Summary"]!.ToString();
        var version = downloadNode["SummaryInfo"]!["Version"]!.ToString();

        var filesNode = downloadNode["Files"]!.AsArray();
        var mainFileNode = filesNode.FirstOrDefault(n => n!["TypeString"]!.ToString().Equals("exe", StringComparison.InvariantCultureIgnoreCase))
                           ?? filesNode.FirstOrDefault(n => n!["TypeString"]!.ToString().Equals("zip", StringComparison.InvariantCultureIgnoreCase))
                           ?? filesNode.FirstOrDefault();

        if (mainFileNode is null)
            return null;

        var fileLocation = mainFileNode["URL"]!.ToString();
        var fileName = new Uri(fileLocation).Segments.LastOrDefault("file");
        var fileSize = mainFileNode["Size"]!.ToString();
        var fileCrc = mainFileNode["SHA256"]?.ToString();
        var releaseDateUnix = long.Parse(mainFileNode["Date"]!["Unix"]!.ToString());
        var releaseDate = DateTimeOffset.FromUnixTimeMilliseconds(releaseDateUnix).DateTime;

        var readmeFileNode = filesNode.FirstOrDefault(n => n!["TypeString"]!.ToString().Equals("txt readme", StringComparison.InvariantCultureIgnoreCase))
                              ?? filesNode.FirstOrDefault(n => n!["TypeString"]!.ToString().Equals("html", StringComparison.InvariantCultureIgnoreCase));

        var readme = readmeFileNode?["URL"]?.ToString();

        return new()
        {
            Id = id,
            Title = title,
            Description = title == description ? string.Empty : description,
            Version = version,
            Category = category,
            FileName = fileName,
            FileSize = fileSize,
            FileCrc = fileCrc,
            ReleaseDate = releaseDate,
            Readme = readme,
            FileLocation = fileLocation,
        };
    }

    private static bool IsCompatible(JsonNode? downloadNode, string osString)
    {
        var operatingSystems = downloadNode?["OperatingSystemKeys"]?.AsArray();

        if (operatingSystems is null || operatingSystems.IsEmpty())
            return true;

        foreach (var operatingSystem in operatingSystems)
            if (operatingSystem is not null && operatingSystem.ToString().StartsWith(osString, StringComparison.CurrentCultureIgnoreCase))
                return true;

        return false;
    }
}
