using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class DisplaySettingExtensions
{
    public static string ToExtendedString(this DisplaySetting displaySetting)
    {
        return $"{displaySetting.Resolution.Width}x{displaySetting.Resolution.Height}{(displaySetting.IsInterlaced ? "i" : "p")} @ {displaySetting.Frequency}Hz @ {displaySetting.ColorDepth} ({displaySetting.Position}, {displaySetting.Orientation}, {displaySetting.OutputScalingMode})";
    }
}