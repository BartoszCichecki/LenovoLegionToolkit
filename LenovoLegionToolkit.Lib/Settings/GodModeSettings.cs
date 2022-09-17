using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Settings
{
    public class GodModeSettings : AbstractSettings<GodModeSettings.GodModeSettingsStore>
    {
        public class GodModeSettingsStore
        {
            public StepperValue? CPULongTermPowerLimit { get; set; }
            public StepperValue? CPUShortTermPowerLimit { get; set; }
            public StepperValue? GPUPowerBoost { get; set; }
            public StepperValue? GPUConfigurableTGP { get; set; }
            public FanTable? FanTable { get; set; }
            public bool? FanFullSpeed { get; set; }
            public int MaxValueOffset { get; set; }
        }

        public override GodModeSettingsStore Default => new();

        protected override string FileName => "godmode.json";
    }
}
