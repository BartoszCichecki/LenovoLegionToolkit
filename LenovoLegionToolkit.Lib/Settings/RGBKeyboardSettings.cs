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
            State = new(RGBKeyboardBacklightSelectedPreset.Off, new RGBKeyboardBacklightPreset[] {
                new(RGBKeyboardEffect.Static,
                    RBGKeyboardSpeed.Slowest,
                    RGBKeyboardBrightness.Low,
                    new(255,255,255),
                    new(255,255,255),
                    new(255,255,255),
                    new(255,255,255)),
                new(RGBKeyboardEffect.Breath,
                    RBGKeyboardSpeed.Slowest,
                    RGBKeyboardBrightness.Low,
                    new(255,255,255),
                    new(255,255,255),
                    new(255,255,255),
                    new(255,255,255)),
                new(RGBKeyboardEffect.Smooth,
                    RBGKeyboardSpeed.Slowest,
                    RGBKeyboardBrightness.Low,
                    new(0,0,0),
                    new(0,0,0),
                    new(0,0,0),
                    new(0,0,0)),
            }),
        };
    }
}
