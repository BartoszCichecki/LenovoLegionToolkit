using System.Collections.Generic;
using System.Drawing.Imaging;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class LogoInfoFormatExtensions
{
    public static IEnumerable<ImageFormat> ImageFormats(this BootLogoFormat format)
    {
        if (format.HasFlag(BootLogoFormat.Bmp))
            yield return ImageFormat.Bmp;
        if (format.HasFlag(BootLogoFormat.Jpeg))
            yield return ImageFormat.Jpeg;
        if (format.HasFlag(BootLogoFormat.Png))
            yield return ImageFormat.Png;
    }

    public static IEnumerable<string> ExtensionFilters(this BootLogoFormat format)
    {
        if (format.HasFlag(BootLogoFormat.Bmp))
            yield return "*.bmp";
        if (format.HasFlag(BootLogoFormat.Png))
            yield return "*.png";
        if (format.HasFlag(BootLogoFormat.Jpeg))
        {
            yield return "*.jpeg";
            yield return "*.jpg";
        }
    }
}
