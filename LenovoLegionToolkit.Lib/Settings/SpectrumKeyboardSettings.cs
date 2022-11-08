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
                        SpectrumKeyboardBacklightProfile.One,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.Always, SpectrumKeyboardBacklightSpeed.None, SpectrumKeyboardBacklightDirection.None, new RGBColor[] { new(255, 255, 255) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardBacklightProfile.Two,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.Always, SpectrumKeyboardBacklightSpeed.None, SpectrumKeyboardBacklightDirection.None, new RGBColor[] { new(255, 0, 0) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardBacklightProfile.Three,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.Always, SpectrumKeyboardBacklightSpeed.None, SpectrumKeyboardBacklightDirection.None, new RGBColor[] { new(0, 255, 0) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardBacklightProfile.Four,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.Always, SpectrumKeyboardBacklightSpeed.None, SpectrumKeyboardBacklightDirection.None, new RGBColor[] { new(0, 0, 255) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardBacklightProfile.Five,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.RainbowWave, SpectrumKeyboardBacklightSpeed.Speed2, SpectrumKeyboardBacklightDirection.BottomToTop, Array.Empty<RGBColor>(), Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardBacklightProfile.Six,
                        new(new SpectrumKeyboardBacklightEffect(SpectrumKeyboardBacklightEffectType.RainbowScrew, SpectrumKeyboardBacklightSpeed.Speed2, SpectrumKeyboardBacklightDirection.Clockwise, Array.Empty<RGBColor>(), Array.Empty<ushort>()))
                    }
                }
            }
        };
    }
}
