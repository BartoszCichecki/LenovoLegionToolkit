namespace LenovoLegionToolkit.Lib.Settings;

public class SpectrumKeyboardSettings : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>
{
    public class SpectrumKeyboardSettingsStore
    {
        public KeyboardLayout? KeyboardLayout { get; set; }
    }

    public SpectrumKeyboardSettings() : base("spectrum_keyboard.json") { }
}