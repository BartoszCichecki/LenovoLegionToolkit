using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;

public class PackageDownloaderSettings()
    : AbstractSettings<PackageDownloaderSettings.PackageDownloaderSettingsStore>("package_downloader.json")
{
    public class PackageDownloaderSettingsStore
    {
        public string? DownloadPath { get; set; }
        public bool OnlyShowUpdates { get; set; }
        public HashSet<string> HiddenPackages { get; set; } = [];
    }
}
