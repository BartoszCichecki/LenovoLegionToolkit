using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;

public class PackageDownloaderSettings : AbstractSettings<PackageDownloaderSettings.PackageDownloaderSettingsStore>
{
    public class PackageDownloaderSettingsStore
    {
        public string? DownloadPath { get; set; }
        public bool OnlyShowUpdates { get; set; }
        public HashSet<string> HiddenPackages { get; set; } = new();
    }

    protected override string FileName => "package_downloader.json";

    protected override PackageDownloaderSettingsStore Default => new();
}