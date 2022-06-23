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
            State = new(RGBKeyboardBacklightPreset.Off, new Dictionary<RGBKeyboardBacklightPreset, RGBKeyboardBacklightSettings> {
                 { RGBKeyboardBacklightPreset.One, new(RGBKeyboardEffect.Static, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low, new(255, 255, 255), new(255, 255, 255), new(255, 255, 255), new(255, 255, 255)) },
                 { RGBKeyboardBacklightPreset.Two, new(RGBKeyboardEffect.Breath, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low, new(255, 255, 255), new(255,255,255), new(255,255,255), new (255,255,255)) },
                 { RGBKeyboardBacklightPreset.Three, new(RGBKeyboardEffect.Smooth, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low, new(255, 255, 255), new(255,255,255), new(255,255,255), new (255,255,255)) },
            }),
        };
    }
}
