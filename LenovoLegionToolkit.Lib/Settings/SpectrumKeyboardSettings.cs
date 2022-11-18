namespace LenovoLegionToolkit.Lib.Settings
{
    public class SpectrumKeyboardSettings : AbstractSettings<SpectrumKeyboardSettings.SpectrumKeyboardSettingsStore>
    {
        public class SpectrumKeyboardSettingsStore
        {
            public KeyboardLayout? KeyboardLayout { get; set; }
        }

        protected override string FileName => "spectrum_keyboard.json";

        public override SpectrumKeyboardSettingsStore Default => new();
    }
}
