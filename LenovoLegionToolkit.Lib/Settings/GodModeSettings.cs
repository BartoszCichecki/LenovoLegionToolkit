using System;
using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings;


public class GodModeSettings : AbstractSettings<GodModeSettings.GodModeSettingsStore>
{
    public class GodModeSettingsStore
    {
        public Guid? ActivePresetId { get; set; }

        public List<GodModeSettingsPreset> Presets { get; set; } = new();

        public class GodModeSettingsPreset
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public StepperValue? CPULongTermPowerLimit { get; set; }
            public StepperValue? CPUShortTermPowerLimit { get; set; }
            public StepperValue? CPUCrossLoadingPowerLimit { get; set; }
            public StepperValue? CPUTemperatureLimit { get; set; }
            public StepperValue? GPUPowerBoost { get; set; }
            public StepperValue? GPUConfigurableTGP { get; set; }
            public StepperValue? GPUTemperatureLimit { get; set; }
            public FanTable? FanTable { get; set; }
            public bool? FanFullSpeed { get; set; }
            public int MaxValueOffset { get; set; }
        }
    }

    protected override GodModeSettingsStore Default => new();

    protected override string FileName => "godmode.json";

    public override GodModeSettingsStore LoadStore() => base.LoadStore() ?? LoadLegacyStore();

    private GodModeSettingsStore LoadLegacyStore()
    {
        var store = Default;
        var legacySettings = new LegacyGodModeSettings();
        var legacyStore = legacySettings.LoadStore();
        if (legacyStore is not null)
        {
            var preset = new GodModeSettingsStore.GodModeSettingsPreset
            {
                Id = Guid.NewGuid(),
                Name = "Default",
                CPULongTermPowerLimit = legacyStore.CPULongTermPowerLimit,
                CPUShortTermPowerLimit = legacyStore.CPUShortTermPowerLimit,
                CPUCrossLoadingPowerLimit = legacyStore.CPUCrossLoadingPowerLimit,
                CPUTemperatureLimit = legacyStore.CPUTemperatureLimit,
                GPUPowerBoost = legacyStore.GPUPowerBoost,
                GPUConfigurableTGP = legacyStore.GPUConfigurableTGP,
                GPUTemperatureLimit = legacyStore.GPUTemperatureLimit,
                FanTable = legacyStore.FanTable,
                FanFullSpeed = legacyStore.FanFullSpeed,
                MaxValueOffset = legacyStore.MaxValueOffset
            };

            store.ActivePresetId = preset.Id;
            store.Presets.Add(preset);
        }

        return store;
    }
}

class LegacyGodModeSettings : AbstractSettings<LegacyGodModeSettings.LegacyGodModeSettingsStore>
{
    public class LegacyGodModeSettingsStore
    {
        public StepperValue? CPULongTermPowerLimit { get; set; }
        public StepperValue? CPUShortTermPowerLimit { get; set; }
        public StepperValue? CPUCrossLoadingPowerLimit { get; set; }
        public StepperValue? CPUTemperatureLimit { get; set; }
        public StepperValue? GPUPowerBoost { get; set; }
        public StepperValue? GPUConfigurableTGP { get; set; }
        public StepperValue? GPUTemperatureLimit { get; set; }
        public FanTable? FanTable { get; set; }
        public bool? FanFullSpeed { get; set; }
        public int MaxValueOffset { get; set; }
    }

    protected override LegacyGodModeSettingsStore Default => new();

    protected override string FileName => "godmode.json";
}