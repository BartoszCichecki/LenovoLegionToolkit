namespace LenovoLegionToolkit.Lib.Settings;

public class SpectrumKeyboardSettings()
    : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>("spectrum_keyboard.json")
{
    public class SpectrumKeyboardSettingsStore
    {
        public KeyboardLayout? KeyboardLayout { get; set; }
    }
}
