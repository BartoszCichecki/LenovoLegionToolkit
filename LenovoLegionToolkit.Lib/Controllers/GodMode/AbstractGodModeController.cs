﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public abstract class AbstractGodModeController : IGodModeController
{
    protected readonly GodModeSettings Settings;
    protected readonly LegionZone LegionZone;

    protected AbstractGodModeController(GodModeSettings settings, LegionZone legionZone)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        LegionZone = legionZone ?? throw new ArgumentNullException(nameof(legionZone));
    }

    public async Task<GodModeState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var store = Settings.Store;
        var defaultState = await GetDefaultStateAsync().ConfigureAwait(false);

        if (!IsValidStore(store))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Loading default state...");

            var id = Guid.NewGuid();
            return new GodModeState
            {
                ActivePresetId = id,
                Presets = new Dictionary<Guid, GodModePreset> { { id, defaultState } }.AsReadOnlyDictionary()
            };
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Loading state from store...");

        return LoadStateFromStore(store, defaultState);
    }

    public Task SetStateAsync(GodModeState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Settings state: {state}");

        var activePresetId = state.ActivePresetId;
        var presets = new Dictionary<Guid, GodModeSettings.GodModeSettingsStore.Preset>();

        foreach (var (id, preset) in state.Presets)
        {
            presets.Add(id, new()
            {
                Name = preset.Name,
                CPULongTermPowerLimit = preset.CPULongTermPowerLimit,
                CPUShortTermPowerLimit = preset.CPUShortTermPowerLimit,
                CPUPeakPowerLimit = preset.CPUPeakPowerLimit,
                CPUCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit,
                APUsPPTPowerLimit = preset.APUsPPTPowerLimit,
                CPUTemperatureLimit = preset.CPUTemperatureLimit,
                GPUPowerBoost = preset.GPUPowerBoost,
                GPUConfigurableTGP = preset.GPUConfigurableTGP,
                GPUTemperatureLimit = preset.GPUTemperatureLimit,
                FanTable = preset.FanTableInfo?.Table,
                FanFullSpeed = preset.FanFullSpeed,
                MaxValueOffset = preset.MaxValueOffset,
            });
        }

        Settings.Store.ActivePresetId = activePresetId;
        Settings.Store.Presets = presets;
        Settings.SynchronizeStore();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State saved.");

        return Task.CompletedTask;
    }

    public abstract Task ApplyStateAsync();

    protected abstract Task<GodModePreset> GetDefaultStateAsync();

    protected bool IsValidStore(GodModeSettings.GodModeSettingsStore store) => store.Presets.Any() && store.Presets.ContainsKey(store.ActivePresetId);

    protected GodModeState LoadStateFromStore(GodModeSettings.GodModeSettingsStore store, GodModePreset defaultState)
    {
        var states = new Dictionary<Guid, GodModePreset>();

        foreach (var (id, preset) in store.Presets)
        {
            states.Add(id, new GodModePreset
            {
                Name = preset.Name,
                CPULongTermPowerLimit = CreateStepperValue(defaultState.CPULongTermPowerLimit, preset.CPULongTermPowerLimit, preset.MaxValueOffset),
                CPUShortTermPowerLimit = CreateStepperValue(defaultState.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit, preset.MaxValueOffset),
                CPUPeakPowerLimit = CreateStepperValue(defaultState.CPUPeakPowerLimit, preset.CPUPeakPowerLimit, preset.MaxValueOffset),
                CPUCrossLoadingPowerLimit = CreateStepperValue(defaultState.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit, preset.MaxValueOffset),
                APUsPPTPowerLimit = CreateStepperValue(defaultState.APUsPPTPowerLimit, preset.APUsPPTPowerLimit, preset.MaxValueOffset),
                CPUTemperatureLimit = CreateStepperValue(defaultState.CPUTemperatureLimit, preset.CPUTemperatureLimit, preset.MaxValueOffset),
                GPUPowerBoost = CreateStepperValue(defaultState.GPUPowerBoost, preset.GPUPowerBoost, preset.MaxValueOffset),
                GPUConfigurableTGP = CreateStepperValue(defaultState.GPUConfigurableTGP, preset.GPUConfigurableTGP, preset.MaxValueOffset),
                GPUTemperatureLimit = CreateStepperValue(defaultState.GPUTemperatureLimit, preset.GPUTemperatureLimit, preset.MaxValueOffset),
                FanTableInfo = GetFanTableInfo(preset, defaultState.FanTableInfo?.Data),
                FanFullSpeed = preset.FanFullSpeed,
                MaxValueOffset = preset.MaxValueOffset
            });
        }

        return new GodModeState
        {
            ActivePresetId = store.ActivePresetId,
            Presets = states.AsReadOnlyDictionary()
        };
    }

    protected static StepperValue? CreateStepperValue(StepperValue? state, StepperValue? store = null, int? maxValueOffset = 0)
    {
        if (!state.HasValue || state.Value.Min == state.Value.Max + (maxValueOffset ?? 0))
            return null;

        return new StepperValue(store?.Value ?? state.Value.Value, state.Value.Min, state.Value.Max, state.Value.Step, Array.Empty<int>(), state.Value.DefaultValue);
    }

    private FanTableInfo? GetFanTableInfo(GodModeSettings.GodModeSettingsStore.Preset preset, FanTableData[]? fanTableData)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting fan table info...");

        if (fanTableData is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Fan table data is null");
            return null;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table data retrieved: {fanTableData}");

        var fanTable = preset.FanTable ?? FanTable.Default;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table retrieved: {fanTable}");

        if (!fanTable.IsValid())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Fan table invalid, replacing with default...");

            fanTable = FanTable.Default;
        }

        return new FanTableInfo(fanTableData, fanTable);
    }
}
