using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;

public class RGBKeyboardSettings() : AbstractSettings<RGBKeyboardSettings.RGBKeyboardSettingsStore>("rgb_keyboard.json")
{
    public class RGBKeyboardSettingsStore
    {
        public RGBKeyboardBacklightState State { get; set; }
    }

    protected override RGBKeyboardSettingsStore Default => new()
    {
        State = new(RGBKeyboardBacklightPreset.Off, new Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> {
            { RGBKeyboardBacklightPreset.One, new(RGBKeyboardBacklightEffect.Static, RGBKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, RGBColor.Green, RGBColor.Teal, RGBColor.Purple, RGBColor.Pink) },
            { RGBKeyboardBacklightPreset.Two, new(RGBKeyboardBacklightEffect.Static, RGBKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, RGBColor.Red, RGBColor.Red, RGBColor.Red, RGBColor.Red) },
            { RGBKeyboardBacklightPreset.Three, new(RGBKeyboardBacklightEffect.Breath, RGBKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low,  RGBColor.White,RGBColor.White,RGBColor.White,RGBColor.White) },
            { RGBKeyboardBacklightPreset.Four, new(RGBKeyboardBacklightEffect.Smooth, RGBKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, RGBColor.White,RGBColor.White,RGBColor.White,RGBColor.White) },
        }),
    };
}
