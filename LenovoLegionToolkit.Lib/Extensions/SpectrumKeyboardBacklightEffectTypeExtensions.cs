namespace LenovoLegionToolkit.Lib.Extensions;

public static class SpectrumKeyboardBacklightEffectTypeExtensions
{

    public static bool IsAllLightsEffect(this SpectrumKeyboardBacklightEffectType type) => type switch
    {
        SpectrumKeyboardBacklightEffectType.AudioBounce => true,
        SpectrumKeyboardBacklightEffectType.AudioRipple => true,
        SpectrumKeyboardBacklightEffectType.AuroraSync => true,
        _ => false
    };

    public static bool IsWholeKeyboardEffect(this SpectrumKeyboardBacklightEffectType type) => type switch
    {
        SpectrumKeyboardBacklightEffectType.Type => true,
        SpectrumKeyboardBacklightEffectType.Ripple => true,
        _ => false
    };
}