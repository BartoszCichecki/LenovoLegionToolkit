using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Settings;


public class GodModeSettings : AbstractSettings<GodModeSettings.GodModeSettingsStore>
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
            public FanTable? FanTable { get; init; }
            public bool? FanFullSpeed { get; init; }
            public int? MaxValueOffset { get; init; }
        }

        public Guid ActivePresetId { get; set; }

        public Dictionary<Guid, Preset> Presets { get; set; } = new();
    }

    public GodModeSettings() : base("godmode.json") { }

    public override GodModeSettingsStore LoadStore() => base.LoadStore() ?? LoadLegacyStore();

    private GodModeSettingsStore LoadLegacyStore()
    {
        var store = Default;
        var legacySettings = new LegacyGodModeSettings();
        var legacyStore = legacySettings.LoadStore();
        if (legacyStore is not null)
        {
            var id = Guid.NewGuid();
            var preset = new GodModeSettingsStore.Preset
            {
                Name = "Default",
                CPULongTermPowerLimit = legacyStore.CPULongTermPowerLimit,
                CPUShortTermPowerLimit = legacyStore.CPUShortTermPowerLimit,
                CPUCrossLoadingPowerLimit = legacyStore.CPUCrossLoadingPowerLimit,
                CPUTemperatureLimit = legacyStore.CPUTemperatureLimit,
                GPUPowerBoost = legacyStore.GPUPowerBoost,
                GPUConfigurableTGP = legacyStore.GPUConfigurableTGP,
                GPUTemperatureLimit = legacyStore.GPUTemperatureLimit,
                FanTable = legacyStore.FanTable,
                FanFullSpeed = legacyStore.FanFullSpeed ?? false,
                MaxValueOffset = legacyStore.MaxValueOffset
            };

            store.ActivePresetId = id;
            store.Presets.Add(id, preset);
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

    public LegacyGodModeSettings() : base("godmode.json")
    {
        JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
    }
}