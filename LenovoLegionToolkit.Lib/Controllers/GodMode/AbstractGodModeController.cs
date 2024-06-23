using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public abstract class AbstractGodModeController(GodModeSettings settings)
    : IGodModeController
{
    public event EventHandler<Guid>? PresetChanged;

    public abstract Task<bool> NeedsVantageDisabledAsync();

    public abstract Task<bool> NeedsLegionZoneDisabledAsync();

    public Task<Guid> GetActivePresetIdAsync() => Task.FromResult(settings.Store.ActivePresetId);

    public Task<string?> GetActivePresetNameAsync()
    {
        var store = settings.Store;
        var name = store.Presets
            .Where(p => p.Key == store.ActivePresetId)
            .Select(p => p.Value.Name)
            .FirstOrDefault();
        return Task.FromResult(name);
    }

    public async Task<GodModeState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var store = settings.Store;
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

        return await LoadStateFromStoreAsync(store, defaultState).ConfigureAwait(false);
    }

    public Task SetStateAsync(GodModeState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state...");

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
                CPUPL1Tau = preset.CPUPL1Tau,
                APUsPPTPowerLimit = preset.APUsPPTPowerLimit,
                CPUTemperatureLimit = preset.CPUTemperatureLimit,
                GPUPowerBoost = preset.GPUPowerBoost,
                GPUConfigurableTGP = preset.GPUConfigurableTGP,
                GPUTemperatureLimit = preset.GPUTemperatureLimit,
                GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline,
                GPUToCPUDynamicBoost = preset.GPUToCPUDynamicBoost,
                FanTable = preset.FanTableInfo?.Table,
                FanFullSpeed = preset.FanFullSpeed,
                MinValueOffset = preset.MinValueOffset,
                MaxValueOffset = preset.MaxValueOffset,
            });
        }

        settings.Store.ActivePresetId = activePresetId;
        settings.Store.Presets = presets;
        settings.SynchronizeStore();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State saved.");

        return Task.CompletedTask;
    }

    public abstract Task ApplyStateAsync();

    public Task<FanTable> GetDefaultFanTableAsync()
    {
        var fanTable = new FanTable([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
        return Task.FromResult(fanTable);
    }

    public abstract Task<FanTable> GetMinimumFanTableAsync();

    public abstract Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync();

    public abstract Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState state);

    protected abstract Task<GodModePreset> GetDefaultStateAsync();

    protected void RaisePresetChanged(Guid presetId) => PresetChanged?.Invoke(this, presetId);

    protected async Task<(Guid, GodModeSettings.GodModeSettingsStore.Preset)> GetActivePresetAsync()
    {
        if (!IsValidStore(settings.Store))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Invalid store, generating default one.");

            var state = await GetStateAsync().ConfigureAwait(false);
            await SetStateAsync(state).ConfigureAwait(false);
        }

        var activePresetId = settings.Store.ActivePresetId;
        var presets = settings.Store.Presets;

        if (presets.TryGetValue(activePresetId, out var activePreset))
            return (activePresetId, activePreset);

        throw new InvalidOperationException($"Preset with ID {activePresetId} not found");
    }

    protected async Task<bool> IsValidFanTableAsync(FanTable fanTable)
    {
        var minimumFanTable = await GetMinimumFanTableAsync().ConfigureAwait(false);
        var minimum = minimumFanTable.GetTable();
        return fanTable.GetTable().Where((t, i) => t < minimum[i] || t > 10u).IsEmpty();
    }

    private static bool IsValidStore(GodModeSettings.GodModeSettingsStore store) => store.Presets.Count != 0 && store.Presets.ContainsKey(store.ActivePresetId);

    private async Task<GodModeState> LoadStateFromStoreAsync(GodModeSettings.GodModeSettingsStore store, GodModePreset defaultState)
    {
        var states = new Dictionary<Guid, GodModePreset>();

        foreach (var (id, preset) in store.Presets)
        {
            states.Add(id, new GodModePreset
            {
                Name = preset.Name,
                CPULongTermPowerLimit = CreateStepperValue(defaultState.CPULongTermPowerLimit, preset.CPULongTermPowerLimit, preset.MinValueOffset, preset.MaxValueOffset),
                CPUShortTermPowerLimit = CreateStepperValue(defaultState.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit, preset.MinValueOffset, preset.MaxValueOffset),
                CPUPeakPowerLimit = CreateStepperValue(defaultState.CPUPeakPowerLimit, preset.CPUPeakPowerLimit, preset.MinValueOffset, preset.MaxValueOffset),
                CPUCrossLoadingPowerLimit = CreateStepperValue(defaultState.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit, preset.MinValueOffset, preset.MaxValueOffset),
                CPUPL1Tau = CreateStepperValue(defaultState.CPUPL1Tau, preset.CPUPL1Tau, preset.MinValueOffset, preset.MaxValueOffset),
                APUsPPTPowerLimit = CreateStepperValue(defaultState.APUsPPTPowerLimit, preset.APUsPPTPowerLimit, preset.MinValueOffset, preset.MaxValueOffset),
                CPUTemperatureLimit = CreateStepperValue(defaultState.CPUTemperatureLimit, preset.CPUTemperatureLimit, preset.MinValueOffset, preset.MaxValueOffset),
                GPUPowerBoost = CreateStepperValue(defaultState.GPUPowerBoost, preset.GPUPowerBoost, preset.MinValueOffset, preset.MaxValueOffset),
                GPUConfigurableTGP = CreateStepperValue(defaultState.GPUConfigurableTGP, preset.GPUConfigurableTGP, preset.MinValueOffset, preset.MaxValueOffset),
                GPUTemperatureLimit = CreateStepperValue(defaultState.GPUTemperatureLimit, preset.GPUTemperatureLimit, preset.MinValueOffset, preset.MaxValueOffset),
                GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = CreateStepperValue(defaultState.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline,
                    preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline,
                    preset.MinValueOffset,
                    preset.MaxValueOffset),
                GPUToCPUDynamicBoost = CreateStepperValue(defaultState.GPUToCPUDynamicBoost, preset.GPUToCPUDynamicBoost),
                FanTableInfo = await GetFanTableInfoAsync(preset, defaultState.FanTableInfo?.Data).ConfigureAwait(false),
                FanFullSpeed = preset.FanFullSpeed,
                MinValueOffset = preset.MinValueOffset ?? defaultState.MinValueOffset,
                MaxValueOffset = preset.MaxValueOffset ?? defaultState.MaxValueOffset
            });
        }

        return new GodModeState
        {
            ActivePresetId = store.ActivePresetId,
            Presets = states.AsReadOnlyDictionary()
        };
    }

    private static StepperValue? CreateStepperValue(StepperValue? state, StepperValue? store = null, int? minValueOffset = 0, int? maxValueOffset = 0)
    {
        if (state is not { } stateValue)
            return null;

        if (stateValue.Steps.Length > 0)
        {
            var value = store?.Value ?? stateValue.Value;
            var steps = stateValue.Steps;
            var defaultValue = stateValue.DefaultValue;

            if (!steps.Contains(value))
            {
                var valueTemp = value;
                value = steps.MinBy(v => Math.Abs((long)v - valueTemp));
            }

            return new(value, 0, 0, 0, steps, defaultValue);
        }

        if (stateValue.Step > 0)
        {
            var value = store?.Value ?? stateValue.Value;
            var min = Math.Max(0, stateValue.Min + (minValueOffset ?? 0));
            var max = stateValue.Max + (maxValueOffset ?? 0);
            var step = stateValue.Step;
            var defaultValue = stateValue.DefaultValue;

            value = MathExtensions.RoundNearest(value, step);

            if (value < min || value > max)
                value = defaultValue ?? Math.Clamp(value, min, max);

            return new(value, min, max, step, [], defaultValue);
        }

        return null;
    }

    private async Task<FanTableInfo?> GetFanTableInfoAsync(GodModeSettings.GodModeSettingsStore.Preset preset, FanTableData[]? fanTableData)
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

        var fanTable = preset.FanTable ?? await GetDefaultFanTableAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table retrieved: {fanTable}");

        if (!await IsValidFanTableAsync(fanTable).ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Fan table invalid, replacing with default...");

            fanTable = await GetDefaultFanTableAsync().ConfigureAwait(false);
        }

        return new FanTableInfo(fanTableData, fanTable);
    }
}
