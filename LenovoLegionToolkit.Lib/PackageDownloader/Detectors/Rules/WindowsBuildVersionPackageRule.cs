using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.PackageDownloader.Detectors.Rules;

internal readonly struct WindowsBuildVersionPackageRule : IPackageRule
{
    private int Version { get; init; }

    public static bool TryCreate(XmlNode? node, out WindowsBuildVersionPackageRule value)
    {
        var versionString = node?.SelectSingleNode("AddressWidth")?.InnerText;

        if (versionString is null || !int.TryParse(RemoveNonVersionCharacters(versionString), out var version))
        {
            value = default;
            return false;
        }

        value = new WindowsBuildVersionPackageRule { Version = version };
        return true;
    }

    public Task<bool> CheckDependenciesSatisfiedAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => CheckBuildNumberAsync();

    public Task<bool> DetectInstallNeededAsync(List<DriverInfo> _1, HttpClient _2, CancellationToken _3) => CheckBuildNumberAsync();

    private async Task<bool> CheckBuildNumberAsync()
    {
        var buildNumbers = await WMI.ReadAsync("root\\CIMV2", $"SELECT * FROM Win32_OperatingSystem", pdc =>
        {
            var buildNumberString = pdc["BuildNumber"].Value.ToString();

            var buildNumber = 0;
            if (int.TryParse(buildNumberString, out var bn))
                buildNumber = bn;

            return buildNumber;
        }).ConfigureAwait(false);

        var buildNumber = buildNumbers.FirstOrDefault();

        var result = Version <= buildNumber;
        return result;
    }

    private static string RemoveNonVersionCharacters(string? versionString)
    {
        var arr = versionString?.ToCharArray() ?? Array.Empty<char>();
        arr = Array.FindAll(arr, c => char.IsDigit(c) || c == '.');
        return new string(arr);
    }
}
