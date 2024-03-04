namespace LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;

public class WhiteKeyboardLenovoLightingBacklightFeature()
    : AbstractLenovoLightingFeature<WhiteKeyboardBacklightState>(0, 0, 1)
{
    protected override WhiteKeyboardBacklightState FromInternal(int _, int level) => (WhiteKeyboardBacklightState)(level - 1);

    protected override (int stateType, int level) ToInternal(WhiteKeyboardBacklightState state) => (0, (int)(state + 1));
}
