namespace LenovoLegionToolkit.Lib.Settings;

public class SpectrumKeyboardSettings : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>
{
    public class SpectrumKeyboardSettingsStore
    {
        public KeyboardLayout? KeyboardLayout { get; set; }
    }

    protected override string FileName => "spectrum_keyboard.json";

    protected override SpectrumKeyboardSettingsStore Default => new();
}