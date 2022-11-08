using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings
{
    public class RGBKeyboardSettings : AbstractSettings<RGBKeyboardSettings.RGBKeyboardSettingsStore>
    {
        public class RGBKeyboardSettingsStore
        {
            public RGBKeyboardBacklightState State { get; set; }
        }

        protected override string FileName => "rgb_keyboard.json";

        public override RGBKeyboardSettingsStore Default => new()
        {
            State = new(RGBKeyboardBacklightPreset.Off, new Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightBacklightPresetDescription> {
                 { RGBKeyboardBacklightPreset.One, new(RGBKeyboardBacklightEffect.Static, RBGKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, new(142, 255, 0), new(0, 212, 255), new(101, 0, 255), new(186, 0, 255)) },
                 { RGBKeyboardBacklightPreset.Two, new(RGBKeyboardBacklightEffect.Breath, RBGKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, new(255, 255, 255), new(255,255,255), new(255,255,255), new (255,255,255)) },
                 { RGBKeyboardBacklightPreset.Three, new(RGBKeyboardBacklightEffect.Smooth, RBGKeyboardBacklightSpeed.Slowest, RGBKeyboardBacklightBrightness.Low, new(255, 255, 255), new(255,255,255), new(255,255,255), new (255,255,255)) },
            }),
        };
    }
}
