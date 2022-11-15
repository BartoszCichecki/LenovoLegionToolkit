using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.PackageDownloader
{
    public class PCSupportPackageDownloader : AbstractPackageDownloader
    {
        private readonly string _catalogBaseUrl = "https://pcsupport.lenovo.com/us/en/api/v4/downloads/drivers?productId=";

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

            using var httpClient = new HttpClient();

            progress?.Report(0);

            var catalogJson = await httpClient.GetStringAsync($"{_catalogBaseUrl}{machineType}", token).ConfigureAwait(false);
            var catalogJsonNode = JsonNode.Parse(catalogJson);
            var downloadsNode = catalogJsonNode?["body"]?["DownloadItems"]?.AsArray();

            if (downloadsNode is null)
                return new();

            var packages = new List<Package>();
            foreach (var downloadNode in downloadsNode)
            {
                if (!IsCompatible(downloadNode, osString))
                    continue;

                var package = await ParsePackageAsync(httpClient, downloadNode!, token).ConfigureAwait(false);
                if (package is null)
                    continue;

                packages.Add(package.Value);
            }

            return packages;
        }

        private async Task<Package?> ParsePackageAsync(HttpClient httpClient, JsonNode downloadNode, CancellationToken token)
        {
            var id = downloadNode["ID"]!.ToJsonString();
            var category = downloadNode["Category"]!["Name"]!.ToString();
            var title = downloadNode["Title"]!.ToString();
            var description = downloadNode["Summary"]!.ToString();
            var version = downloadNode["SummaryInfo"]!["Version"]!.ToString();

            var mainFileNode = downloadNode["Files"]!.AsArray().FirstOrDefault(n => n!["TypeString"]!.ToString() == "EXE")!;

            if (mainFileNode is null)
                return null;

            var fileLocation = mainFileNode["URL"]!.ToString();
            var fileName = fileLocation[(fileLocation.LastIndexOf('/') + 1)..];
            var fileSize = mainFileNode["Size"]!.ToString();
            var releaseDateUnix = long.Parse(mainFileNode["Date"]!["Unix"]!.ToString());
            var releaseDate = DateTimeOffset.FromUnixTimeMilliseconds(releaseDateUnix).DateTime;

            string? readme = null;
            var readmeFileNode = downloadNode["Files"]!.AsArray().FirstOrDefault(n => n!["TypeString"]!.ToString() == "TXT README");
            if (readmeFileNode is not null)
            {
                var readmeLocation = readmeFileNode["URL"]!.ToString();
                readme = await GetReadmeAsync(httpClient, readmeLocation, token).ConfigureAwait(false);
            }

            return new()
            {
                Id = id,
                Title = title,
                Description = (title == description) ? string.Empty : description,
                Version = version,
                Category = category,
                FileName = fileName,
                FileSize = fileSize,
                ReleaseDate = releaseDate,
                Readme = readme,
                FileLocation = fileLocation,
            };
        }

        private bool IsCompatible(JsonNode? downloadNode, string osString)
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
}
