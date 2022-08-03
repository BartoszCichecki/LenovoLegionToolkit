namespace LenovoLegionToolkit.Lib.Settings
{
    public class GodModeSettings : AbstractSettings<GodModeSettings.GodModeSettingsStore>
    {
        public class GodModeSettingsStore
        {
            public StepperValue? GPUPowerBoost { get; set; }
            public bool? FanCooling { get; set; }
        }

        public override GodModeSettingsStore Default => new();

        protected override string FileName => "godmode.json";
    }
}
