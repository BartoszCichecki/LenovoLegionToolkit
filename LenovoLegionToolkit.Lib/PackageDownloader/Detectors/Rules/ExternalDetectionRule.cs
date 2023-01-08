using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct ExternalDetectionRule : IPackageRule
{
    private const string TEMP_FOLDER_SUB_FOLDER = "external_package_detection";

    private int[] ReturnCodes { get; init; }
    private string Command { get; init; }
    private string Url { get; init; }
    private string FileName { get; init; }
    private string PackageName { get; init; }

    public static bool TryCreate(XmlNode? node, XmlDocument document, string baseLocation, out ExternalDetectionRule value)
    {
        var command = node?.InnerText;
        var returnCodes = node?.Attributes?.OfType<XmlAttribute>()
            .FirstOrDefault(a => a.Name == "rc")?
            .InnerText
            .Split(",")
            .Select(s => int.TryParse(s, out var result) ? result : -1)
            .Where(i => i >= 0)
            .Distinct()
            .ToArray() ?? Array.Empty<int>();
        var externalFile = document.SelectSingleNode("/Package/Files/External/File/Name")?.InnerText;
        var packageName = document.SelectSingleNode("/Package/@id")?.InnerText;

        if (command is null || returnCodes.IsEmpty() || externalFile is null || packageName is null)
        {
            value = default;
            return false;
        }

        value = new ExternalDetectionRule
        {
            Command = command,
            ReturnCodes = returnCodes,
            Url = $"{baseLocation}/{externalFile}",
            FileName = externalFile,
            PackageName = packageName
        };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _, HttpClient httpClient, CancellationToken token) => CheckExternalDependency(httpClient, token);

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _, HttpClient httpClient, CancellationToken token) => CheckExternalDependency(httpClient, token);

    private async Task<bool> CheckExternalDependency(HttpClient httpClient, CancellationToken token)
    {
        var packagePath = Path.Combine(Folders.Temp, TEMP_FOLDER_SUB_FOLDER, PackageName);
        var filePath = Path.Combine(packagePath, FileName);

        if (!Directory.Exists(packagePath))
            Directory.CreateDirectory(packagePath);

        if (!File.Exists(filePath))
        {
            await using var fileStream = File.OpenWrite(filePath);
            await httpClient.DownloadAsync(Url, fileStream, null, token).ConfigureAwait(false);
        }

        var executable = Command.Split(' ').FirstOrDefault();
        var arguments = string.Join(' ', Command.Split(' ').Skip(1));

        if (executable is null)
            return false;

        if (executable.Contains("%PACKAGEPATH%"))
            executable = executable.Replace("%PACKAGEPATH%", packagePath);

        if (!executable.Contains('\\'))
            executable = Path.Combine(packagePath, executable);

        var (exitCode, _) = await CMD.RunAsync(executable, arguments).ConfigureAwait(false);
        var result = ReturnCodes.Contains(exitCode);
        return result;
    }
}
