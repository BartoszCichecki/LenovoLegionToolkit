using System;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class PackageDownloaderFactory(
    PCSupportPackageDownloader pcSupportPackageDownloader,
    VantagePackageDownloader vantagePackageDownloader)
{
    public enum Type
    {
        PCSupport,
        Vantage,
    }

    public IPackageDownloader GetInstance(Type type) => type switch
    {
        Type.PCSupport => pcSupportPackageDownloader,
        Type.Vantage => vantagePackageDownloader,
        _ => throw new InvalidOperationException(nameof(type)),
    };
}
