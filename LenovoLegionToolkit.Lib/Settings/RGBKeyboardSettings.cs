namespace LenovoLegionToolkit.Lib.Settings
{
    public class RGBKeyboardSettings : AbstractSettings<RGBKeyboardSettings.RGBKeyboardSettingsStore>
    {
        public class RGBKeyboardSettingsStore
        {
            public RGBKeyboardBacklightState State { get; set; }
        }

        protected override string FileName => "rgb_keyboard.json";
    }
}
