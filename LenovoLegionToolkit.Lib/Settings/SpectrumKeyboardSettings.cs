using System;

namespace LenovoLegionToolkit.Lib.Settings
{
    public class SpectrumKeyboardSettings : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>
    {
        public class SpectrumKeyboardSettingsStore
        {
            public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.Ansi;
            public SpectrumKeyboardState State { get; set; }
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
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.Always, SpectrumKeyboardSpeed.None, SpectrumKeyboardDirection.None, new RGBColor[] { new(255, 255, 255) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardProfile.Two,
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.Always, SpectrumKeyboardSpeed.None, SpectrumKeyboardDirection.None, new RGBColor[] { new(255, 0, 0) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardProfile.Three,
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.Always, SpectrumKeyboardSpeed.None, SpectrumKeyboardDirection.None, new RGBColor[] { new(0, 255, 0) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardProfile.Four,
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.Always, SpectrumKeyboardSpeed.None, SpectrumKeyboardDirection.None, new RGBColor[] { new(0, 0, 255) }, Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardProfile.Five,
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.RainbowWave, SpectrumKeyboardSpeed.Speed2, SpectrumKeyboardDirection.BottomToTop, Array.Empty<RGBColor>(), Array.Empty<ushort>()))
                    },
                    {
                        SpectrumKeyboardProfile.Six,
                        new(new SpectrumKeyboardEffect(SpectrumKeyboardEffectType.RainbowScrew, SpectrumKeyboardSpeed.Speed2, SpectrumKeyboardDirection.Clockwise, Array.Empty<RGBColor>(), Array.Empty<ushort>()))
                    }
                }
            }
        };
    }
}
