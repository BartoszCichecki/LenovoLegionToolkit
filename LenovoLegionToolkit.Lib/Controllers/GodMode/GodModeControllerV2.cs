﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV2 : AbstractGodModeController
{
    public GodModeControllerV2(GodModeSettings settings, VantageDisabler vantageDisabler, LegionZoneDisabler legionZoneDisabler) : base(settings, vantageDisabler, legionZoneDisabler) { }

    public override Task<bool> NeedsVantageDisabledAsync() => Task.FromResult(true);
    public override Task<bool> NeedsLegionZoneDisabledAsync() => Task.FromResult(true);

    public override async Task ApplyStateAsync()
    {
        if (await VantageDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Vantage is running.");
            return;
        }

        if (await LegionZoneDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var (presetId, preset) = await GetActivePresetAsync().ConfigureAwait(false);

        var settings = new Dictionary<CapabilityID, StepperValue?>
        {
            { CapabilityID.CPULongTermPowerLimit, preset.CPULongTermPowerLimit },
            { CapabilityID.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit },
            { CapabilityID.CPUPeakPowerLimit, preset.CPUPeakPowerLimit },
            { CapabilityID.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit },
            { CapabilityID.CPUPL1Tau, preset.CPUPL1Tau },
            { CapabilityID.APUsPPTPowerLimit, preset.APUsPPTPowerLimit },
            { CapabilityID.CPUTemperatureLimit, preset.CPUTemperatureLimit },
            { CapabilityID.CPUToGPUDynamicBoost, preset.CPUToGPUDynamicBoost },
            { CapabilityID.GPUPowerBoost, preset.GPUPowerBoost },
            { CapabilityID.GPUConfigurableTGP, preset.GPUConfigurableTGP },
            { CapabilityID.GPUTemperatureLimit, preset.GPUTemperatureLimit },
            { CapabilityID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline },
            { CapabilityID.GPUToCPUDynamicBoost, preset.GPUToCPUDynamicBoost },
        };

        var fanTable = preset.FanTable ?? await GetDefaultFanTableAsync().ConfigureAwait(false);
        var fanFullSpeed = preset.FanFullSpeed ?? false;

        foreach (var (id, value) in settings)
        {
            if (!value.HasValue)
                continue;

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying {id}: {value}...");

                await SetValueAsync(id, value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to apply {id}. [value={value}]", ex);
                throw;
            }
        }

        if (fanFullSpeed)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Full Speed {fanFullSpeed}...");

                await SetFanFullSpeedAsync(fanFullSpeed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanFullSpeed]", ex);
                throw;
            }
        }
        else
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Making sure Fan Full Speed is false...");

                await SetFanFullSpeedAsync(false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanFullSpeed]", ex);
                throw;
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Table {fanTable}...");

                if (!await IsValidFanTableAsync(fanTable).ConfigureAwait(false))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    fanTable = await GetDefaultFanTableAsync().ConfigureAwait(false);
                }

                await SetFanTable(fanTable).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=fanTable]", ex);
                throw;
            }
        }

        RaisePresetChanged(presetId);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State applied. [name={preset.Name}, id={presetId}]");
    }

    public override Task<FanTable> GetMinimumFanTableAsync()
    {
        var fanTable = new FanTable(new ushort[] { 1, 1, 1, 1, 1, 1, 1, 1, 3, 5 });
        return Task.FromResult(fanTable);
    }

    public override async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting defaults in other power modes...");

            var result = new Dictionary<PowerModeState, GodModeDefaults>();

            var allCapabilityData = await WMI.LenovoCapabilityData01.ReadAsync().ConfigureAwait(false);
            allCapabilityData = allCapabilityData.ToArray();

            foreach (var powerMode in new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance })
            {
                var defaults = new GodModeDefaults
                {
                    CPULongTermPowerLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPULongTermPowerLimit, powerMode),
                    CPUShortTermPowerLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUShortTermPowerLimit, powerMode),
                    CPUPeakPowerLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUPeakPowerLimit, powerMode),
                    CPUCrossLoadingPowerLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUCrossLoadingPowerLimit, powerMode),
                    CPUPL1Tau = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUPL1Tau, powerMode),
                    APUsPPTPowerLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.APUsPPTPowerLimit, powerMode),
                    CPUTemperatureLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUTemperatureLimit, powerMode),
                    CPUToGPUDynamicBoost = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.CPUToGPUDynamicBoost, powerMode),
                    GPUPowerBoost = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.GPUPowerBoost, powerMode),
                    GPUConfigurableTGP = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.GPUConfigurableTGP, powerMode),
                    GPUTemperatureLimit = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.GPUTemperatureLimit, powerMode),
                    GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline, powerMode),
                    GPUToCPUDynamicBoost = GetDefaultCapabilityIdValueInPowerMode(allCapabilityData, CapabilityID.GPUToCPUDynamicBoost, powerMode),
                    FanTable = await GetDefaultFanTableAsync().ConfigureAwait(false),
                    FanFullSpeed = false
                };

                result[powerMode] = defaults;
            }

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Defaults in other power modes retrieved:");
                foreach (var (powerMode, defaults) in result)
                    Log.Instance.Trace($" - {powerMode}: {defaults}");
            }

            return result;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get defaults in other power modes.", ex);

            return new Dictionary<PowerModeState, GodModeDefaults>();
        }
    }

    public override Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState _) => Task.CompletedTask;

    protected override async Task<GodModePreset> GetDefaultStateAsync()
    {
        var allCapabilityData = await WMI.LenovoCapabilityData01.ReadAsync().ConfigureAwait(false);
        allCapabilityData = allCapabilityData.ToArray();

        var capabilityData = allCapabilityData
            .Where(d => Enum.IsDefined(d.Id))
            .ToArray();

        var allDiscreteData = await WMI.LenovoDiscreteData.ReadAsync().ConfigureAwait(false);
        allDiscreteData = allDiscreteData.ToArray();

        var discreteData = allDiscreteData
            .Where(d => Enum.IsDefined(d.Id))
            .GroupBy(d => d.Id, d => d.Value, (id, values) => (id, values))
            .ToDictionary(d => d.id, d => d.values.ToArray());

        var stepperValues = new Dictionary<CapabilityID, StepperValue>();

        foreach (var c in capabilityData)
        {
            var value = await GetValueAsync(c.Id).OrNullIfException().ConfigureAwait(false) ?? c.DefaultValue;
            var steps = discreteData.GetValueOrDefault(c.Id) ?? Array.Empty<int>();

            if (c.Step == 0 && steps.Length < 1)
                continue;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Creating StepperValue {c.Id}... [idRaw={(int)c.Id:X}, defaultValue={c.DefaultValue}, min={c.Min}, max={c.Max}, step={c.Step}, steps={string.Join(", ", steps)}]");

            var stepperValue = new StepperValue(value, c.Min, c.Max, c.Step, steps, c.DefaultValue);
            stepperValues[c.Id] = stepperValue;
        }

        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = stepperValues.GetValueOrNull(CapabilityID.CPULongTermPowerLimit),
            CPUShortTermPowerLimit = stepperValues.GetValueOrNull(CapabilityID.CPUShortTermPowerLimit),
            CPUPeakPowerLimit = stepperValues.GetValueOrNull(CapabilityID.CPUPeakPowerLimit),
            CPUCrossLoadingPowerLimit = stepperValues.GetValueOrNull(CapabilityID.CPUCrossLoadingPowerLimit),
            CPUPL1Tau = stepperValues.GetValueOrNull(CapabilityID.CPUPL1Tau),
            APUsPPTPowerLimit = stepperValues.GetValueOrNull(CapabilityID.APUsPPTPowerLimit),
            CPUTemperatureLimit = stepperValues.GetValueOrNull(CapabilityID.CPUTemperatureLimit),
            CPUToGPUDynamicBoost = stepperValues.GetValueOrNull(CapabilityID.CPUToGPUDynamicBoost),
            GPUPowerBoost = stepperValues.GetValueOrNull(CapabilityID.GPUPowerBoost),
            GPUConfigurableTGP = stepperValues.GetValueOrNull(CapabilityID.GPUConfigurableTGP),
            GPUTemperatureLimit = stepperValues.GetValueOrNull(CapabilityID.GPUTemperatureLimit),
            GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = stepperValues.GetValueOrNull(CapabilityID.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline),
            GPUToCPUDynamicBoost = stepperValues.GetValueOrNull(CapabilityID.GPUToCPUDynamicBoost),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, await GetDefaultFanTableAsync().ConfigureAwait(false)),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false),
            MinValueOffset = 0,
            MaxValueOffset = 0
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    private static CapabilityID AdjustCapabilityIdForPowerMode(CapabilityID id, PowerModeState powerMode)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        var powerModeRaw = ((uint)powerMode + 1) << 8;
        return (CapabilityID)(idRaw + powerModeRaw);
    }

    private static int? GetDefaultCapabilityIdValueInPowerMode(IEnumerable<RangeCapability> capabilities, CapabilityID id, PowerModeState powerMode)
    {
        var adjustedId = AdjustCapabilityIdForPowerMode(id, powerMode);
        var value = capabilities
            .Where(c => c.Id == adjustedId)
            .Select(c => c.DefaultValue)
            .DefaultIfEmpty(-1)
            .First();
        return value < 0 ? null : value;
    }

    #region Get/Set Value

    private static Task<int> GetValueAsync(CapabilityID id)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.LenovoOtherMethod.GetFeatureValueAsync(idRaw);
    }

    private static Task SetValueAsync(CapabilityID id, StepperValue value)
    {
        var idRaw = (uint)id & 0xFFFF00FF;
        return WMI.LenovoOtherMethod.SetFeatureValueAsync(idRaw, value.Value);
    }

    #endregion

    #region Fan Table

    private static async Task<FanTableData[]?> GetFanTableDataAsync(PowerModeState powerModeState = PowerModeState.GodMode)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Reading fan table data...");

        var data = await WMI.LenovoFanTableData.ReadAsync().ConfigureAwait(false);

        var fanTableData = data
            .Where(d => d.mode == (int)powerModeState + 1)
            .Select(d => new FanTableData
            {
                Type = (d.fanId, d.sensorId) switch
                {
                    (1, 4) => FanTableType.CPU,
                    (1, 1) => FanTableType.CPUSensor,
                    (2, 5) => FanTableType.GPU,
                    _ => FanTableType.Unknown,
                },
                FanId = d.fanId,
                SensorId = d.sensorId,
                FanSpeeds = d.fanTableData,
                Temps = d.sensorTableData
            })
            .ToArray();

        if (fanTableData.Length != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.FanSpeeds.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table fan speeds length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.Temps.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table temps length: {string.Join(", ", fanTableData)}");

            return null;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table data: {string.Join(", ", fanTableData)}");

        return fanTableData;
    }

    private static Task SetFanTable(FanTable fanTable) => WMI.LenovoFanMethod.FanSetTableAsync(fanTable.GetBytes());

    #endregion

    #region Fan Full Speed

    private static async Task<bool> GetFanFullSpeedAsync()
    {
        var value = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.FanFullSpeed).ConfigureAwait(false);
        return value != 0;
    }

    private static Task SetFanFullSpeedAsync(bool enabled) => WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.FanFullSpeed, enabled ? 1 : 0);

    #endregion

}
