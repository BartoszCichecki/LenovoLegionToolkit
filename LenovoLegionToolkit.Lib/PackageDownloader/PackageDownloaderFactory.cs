using System;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class PackageDownloaderFactory
{
    public enum Type
    {
        PCSupport,
        Commercial,
    }

    private readonly PCSupportPackageDownloader _pcSupportPackageDownloader;
    private readonly CommercialPackageDownloader _commercialPackageDownloader;

    public PackageDownloaderFactory(PCSupportPackageDownloader pcSupportPackageDownloader, CommercialPackageDownloader commercialPackageDownloader)
    {
        _pcSupportPackageDownloader = pcSupportPackageDownloader ?? throw new ArgumentNullException(nameof(pcSupportPackageDownloader));
        _commercialPackageDownloader = commercialPackageDownloader ?? throw new ArgumentNullException(nameof(commercialPackageDownloader));
    }

    public IPackageDownloader GetInstance(Type type) => type switch
    {
        Type.PCSupport => _pcSupportPackageDownloader,
        Type.Commercial => _commercialPackageDownloader,
        _ => throw new InvalidOperationException(nameof(type)),
    };
}