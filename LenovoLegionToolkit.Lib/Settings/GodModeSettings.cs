using System;
using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;


public class GodModeSettings() : AbstractSettings<GodModeSettings.GodModeSettingsStore>("godmode.json")
{
    public class GodModeSettingsStore
    {
        public class Preset
        {
            public string Name { get; init; } = string.Empty;
            public StepperValue? CPULongTermPowerLimit { get; init; }
            public StepperValue? CPUShortTermPowerLimit { get; init; }
            public StepperValue? CPUPeakPowerLimit { get; init; }
            public StepperValue? CPUCrossLoadingPowerLimit { get; init; }
            public StepperValue? CPUPL1Tau { get; init; }
            public StepperValue? APUsPPTPowerLimit { get; init; }
            public StepperValue? CPUTemperatureLimit { get; init; }
            public StepperValue? GPUPowerBoost { get; init; }
            public StepperValue? GPUConfigurableTGP { get; init; }
            public StepperValue? GPUTemperatureLimit { get; init; }
            public StepperValue? GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline { get; init; }
            public StepperValue? GPUToCPUDynamicBoost { get; init; }
            public FanTable? FanTable { get; init; }
            public bool? FanFullSpeed { get; init; }
            public int? MinValueOffset { get; init; }
            public int? MaxValueOffset { get; init; }
        }

        public Guid ActivePresetId { get; set; }

        public Dictionary<Guid, Preset> Presets { get; set; } = [];
    }

    // ReSharper disable once StringLiteralTypo
}
