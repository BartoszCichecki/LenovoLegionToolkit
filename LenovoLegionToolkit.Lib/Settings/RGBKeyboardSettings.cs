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
                 { RGBKeyboardBacklightPreset.One, new(RGBKeyboardEffect.Static, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low,
                     new(new(142, 255, 0), false), new(new(0, 212, 255), false), new(new(101, 0, 255), false), new(new(186, 0, 255), false)) },

                 { RGBKeyboardBacklightPreset.Two, new(RGBKeyboardEffect.Breath, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low,
                     new(new(255, 255, 255), false), new(new(255,255,255), false), new(new(255,255,255), false), new(new(255,255,255), false)) },

                 { RGBKeyboardBacklightPreset.Three, new(RGBKeyboardEffect.Smooth, RBGKeyboardSpeed.Slowest, RGBKeyboardBrightness.Low,
                     new(new(255, 255, 255), false), new(new(255,255,255), false), new(new(255,255,255), false), new(new(255,255,255), false)) },
            }),
        };
    }
}
