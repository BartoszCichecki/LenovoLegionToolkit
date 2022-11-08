using System;

namespace LenovoLegionToolkit.Lib.Settings
{
    public class SpectrumKeyboardSettings : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>
    {
        public class SpectrumKeyboardSettingsStore
        {
            public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.Ansi;
            public SpectrumKeyboardBacklightState State { get; set; }
        }

        protected override string FileName => "spectrum_keyboard.json";

        public override SpectrumKeyboardSettingsStore Default => new()
        {
            KeyboardLayout = KeyboardLayout.Ansi,
            State = new()
            {
                Profiles =
                {
                    {
                        SpectrumKeyboardProfile.One,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.Always,
                            SpectrumKeyboardSpeed.None,
                            SpectrumKeyboardDirection.None,
                            Array.Empty<RGBColor>(),
                            new ushort[] { 0x65 }))
                    },
                    {
                        SpectrumKeyboardProfile.Two,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.Always,
                            SpectrumKeyboardSpeed.None,
                            SpectrumKeyboardDirection.None,
                            new RGBColor[] { new(255, 0, 0) },
                            new ushort[] { 0x65 }))
                    },
                    {
                        SpectrumKeyboardProfile.Three,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.Always,
                            SpectrumKeyboardSpeed.None,
                            SpectrumKeyboardDirection.None,
                            new RGBColor[] { new(0, 255, 0) },
                            new ushort[] { 0x65 }))
                    },
                    {
                        SpectrumKeyboardProfile.Four,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.Always,
                            SpectrumKeyboardSpeed.None,
                            SpectrumKeyboardDirection.None,
                            new RGBColor[] { new(0, 0, 255) },
                            new ushort[] { 0x65 }))
                    },
                    {
                        SpectrumKeyboardProfile.Five,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.RainbowWave,
                            SpectrumKeyboardSpeed.Speed2,
                            SpectrumKeyboardDirection.BottomToTop,
                            Array.Empty<RGBColor>(),
                            new ushort[] { 0x65 }))
                    },
                    {
                        SpectrumKeyboardProfile.Six,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardEffect.RainbowScrew,
                            SpectrumKeyboardSpeed.Speed2,
                            SpectrumKeyboardDirection.Clockwise,
                            Array.Empty<RGBColor>(),
                            new ushort[] { 0x65 }))
                    }
                }
            }
        };
    }
}
