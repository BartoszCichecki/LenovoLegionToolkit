namespace LenovoLegionToolkit.Lib.Extensions;

public static class RGBKeyboardBacklightPresetExtensions
{
    public static RGBKeyboardBacklightPreset Next(this RGBKeyboardBacklightPreset preset)
    {
        return preset switch
        {
            RGBKeyboardBacklightPreset.Off => RGBKeyboardBacklightPreset.One,
            RGBKeyboardBacklightPreset.One => RGBKeyboardBacklightPreset.Two,
            RGBKeyboardBacklightPreset.Two => RGBKeyboardBacklightPreset.Three,
            _ => RGBKeyboardBacklightPreset.Off,
        };
    }
}