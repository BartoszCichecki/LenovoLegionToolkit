using System;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class PackageDownloaderFactory
{
    public enum Type
    {
        PCSupport,
        Vantage,
    }

    private readonly PCSupportPackageDownloader _pcSupportPackageDownloader;
    private readonly VantagePackageDownloader _vantagePackageDownloader;

    public PackageDownloaderFactory(PCSupportPackageDownloader pcSupportPackageDownloader, VantagePackageDownloader vantagePackageDownloader)
    {
        _pcSupportPackageDownloader = pcSupportPackageDownloader ?? throw new ArgumentNullException(nameof(pcSupportPackageDownloader));
        _vantagePackageDownloader = vantagePackageDownloader ?? throw new ArgumentNullException(nameof(vantagePackageDownloader));
    }

    public IPackageDownloader GetInstance(Type type) => type switch
    {
        Type.PCSupport => _pcSupportPackageDownloader,
        Type.Vantage => _vantagePackageDownloader,
        _ => throw new InvalidOperationException(nameof(type)),
    };
}