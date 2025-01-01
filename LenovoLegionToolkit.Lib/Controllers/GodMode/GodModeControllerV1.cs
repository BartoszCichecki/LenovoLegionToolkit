using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV1(
    GodModeSettings settings,
    LegionZoneDisabler legionZoneDisabler)
    : AbstractGodModeController(settings)
{
    public override Task<bool> NeedsVantageDisabledAsync() => Task.FromResult(false);
    public override Task<bool> NeedsLegionZoneDisabledAsync() => Task.FromResult(true);

    public override async Task ApplyStateAsync()
    {
        if (await legionZoneDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var (presetId, preset) = await GetActivePresetAsync().ConfigureAwait(false);

        var cpuLongTermPowerLimit = preset.CPULongTermPowerLimit;
        var cpuShortTermPowerLimit = preset.CPUShortTermPowerLimit;
        var cpuPeakPowerLimit = preset.CPUPeakPowerLimit;
        var cpuCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit;
        var apuSPPTPowerLimit = preset.APUsPPTPowerLimit;
        var cpuTemperatureLimit = preset.CPUTemperatureLimit;
        var gpuPowerBoost = preset.GPUPowerBoost;
        var gpuConfigurableTgp = preset.GPUConfigurableTGP;
        var gpuTemperatureLimit = preset.GPUTemperatureLimit;
        var fanTable = preset.FanTable ?? await GetDefaultFanTableAsync().ConfigureAwait(false);
        var fanFullSpeed = preset.FanFullSpeed ?? false;

        if (cpuLongTermPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Long Term Power Limit: {cpuLongTermPowerLimit}");

                await SetCPULongTermPowerLimitAsync(cpuLongTermPowerLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuLongTermPowerLimit]", ex);
                throw;
            }
        }

        if (cpuShortTermPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Short Term Power Limit: {cpuShortTermPowerLimit}");

                await SetCPUShortTermPowerLimitAsync(cpuShortTermPowerLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuShortTermPowerLimit]", ex);
                throw;
            }
        }

        if (cpuPeakPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Peak Power Limit: {cpuPeakPowerLimit}");

                await SetCPUPeakPowerLimitAsync(cpuPeakPowerLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuPeakPowerLimit]", ex);
                throw;
            }
        }

        if (cpuCrossLoadingPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Cross Loading Power Limit: {cpuCrossLoadingPowerLimit}");

                await SetCPUCrossLoadingPowerLimitAsync(cpuCrossLoadingPowerLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuCrossLoadingPowerLimit]", ex);
                throw;
            }
        }

        if (apuSPPTPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying APU sPPT Power Limit: {apuSPPTPowerLimit}");

                await SetAPUSPPTPowerLimitAsync(apuSPPTPowerLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=apuSPPTPowerLimit]", ex);
                throw;
            }
        }

        if (cpuTemperatureLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Temperature Limit: {cpuTemperatureLimit}");

                await SetCPUTemperatureLimitAsync(cpuTemperatureLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuTemperatureLimit]", ex);
                throw;
            }
        }

        if (gpuPowerBoost is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying GPU Power Boost: {gpuPowerBoost}");

                await SetGPUPowerBoostAsync(gpuPowerBoost.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=gpuPowerBoost]", ex);
            }
        }

        if (gpuConfigurableTgp is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying GPU Configurable TGP: {gpuConfigurableTgp}");

                await SetGPUConfigurableTGPAsync(gpuConfigurableTgp.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=gpuConfigurableTgp]", ex);
            }
        }

        if (gpuTemperatureLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying GPU Temperature Limit: {gpuTemperatureLimit}");

                await SetGPUTemperatureLimitAsync(gpuTemperatureLimit.Value.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=gpuTemperatureLimit]", ex);
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
        var fanTable = new FanTable([0, 0, 0, 0, 0, 0, 0, 1, 3, 5]);
        return Task.FromResult(fanTable);
    }

    public override async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting defaults in other power modes...");

            var result = await GetDefaultValuesInDifferentModeAsync().ConfigureAwait(false);

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
            return [];
        }
    }

    public override async Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState state)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Restoring defaults for {state}...");

            var result = await GetDefaultValuesInDifferentModeAsync().ConfigureAwait(false);

            if (!result.TryGetValue(state, out var defaults))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Defaults for {state} not found. Skipping...");

                return;
            }

            if (defaults.CPULongTermPowerLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying CPU Long Term Power Limit: {defaults.CPULongTermPowerLimit}");

                    await SetCPULongTermPowerLimitAsync(defaults.CPULongTermPowerLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=cpuLongTermPowerLimit]", ex);
                    throw;
                }
            }

            if (defaults.CPUShortTermPowerLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying CPU Short Term Power Limit: {defaults.CPUShortTermPowerLimit}");

                    await SetCPUShortTermPowerLimitAsync(defaults.CPUShortTermPowerLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=cpuShortTermPowerLimit]", ex);
                    throw;
                }
            }

            if (defaults.CPUPeakPowerLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying CPU Peak Power Limit: {defaults.CPUPeakPowerLimit}");

                    await SetCPUPeakPowerLimitAsync(defaults.CPUPeakPowerLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=cpuPeakPowerLimit]", ex);
                    throw;
                }
            }

            if (defaults.CPUCrossLoadingPowerLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying CPU Cross Loading Power Limit: {defaults.CPUCrossLoadingPowerLimit}");

                    await SetCPUCrossLoadingPowerLimitAsync(defaults.CPUCrossLoadingPowerLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=cpuCrossLoadingPowerLimit]", ex);
                    throw;
                }
            }

            if (defaults.APUsPPTPowerLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying APU sPPT Power Limit: {defaults.APUsPPTPowerLimit}");

                    await SetAPUSPPTPowerLimitAsync(defaults.APUsPPTPowerLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=apuSPPTPowerLimit]", ex);
                    throw;
                }
            }

            if (defaults.CPUTemperatureLimit is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying CPU Temperature Limit: {defaults.CPUTemperatureLimit}");

                    await SetCPUTemperatureLimitAsync(defaults.CPUTemperatureLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=cpuTemperatureLimit]", ex);
                    throw;
                }
            }

            if (defaults.GPUPowerBoost is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying GPU Power Boost: {defaults.GPUPowerBoost}");

                    await SetGPUPowerBoostAsync(defaults.GPUPowerBoost.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=gpuPowerBoost]", ex);
                    throw;
                }
            }

            if (defaults.GPUConfigurableTGP is not null)
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Applying GPU Configurable TGP: {defaults.GPUConfigurableTGP}");

                    await SetGPUConfigurableTGPAsync(defaults.GPUConfigurableTGP.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=gpuConfigurableTgp]", ex);
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to restore defaults for {state}.", ex);
        }
    }

    protected override async Task<GodModePreset> GetDefaultStateAsync()
    {
        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);

        var preset = new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = await GetCPULongTermPowerLimitAsync().OrNullIfException().ConfigureAwait(false),
            CPUShortTermPowerLimit = await GetCPUShortTermPowerLimitAsync().OrNullIfException().ConfigureAwait(false),
            CPUPeakPowerLimit = await GetCPUPeakPowerLimitAsync().OrNullIfException().ConfigureAwait(false),
            CPUCrossLoadingPowerLimit = await GetCPUCrossLoadingPowerLimitAsync().OrNullIfException().ConfigureAwait(false),
            APUsPPTPowerLimit = await GetAPUSPPTPowerLimitAsync().OrNullIfException().ConfigureAwait(false),
            CPUTemperatureLimit = await GetCPUTemperatureLimitAsync().OrNullIfException().ConfigureAwait(false),
            GPUPowerBoost = await GetGPUPowerBoost().OrNullIfException().ConfigureAwait(false),
            GPUConfigurableTGP = await GetGPUConfigurableTGPAsync().OrNullIfException().ConfigureAwait(false),
            GPUTemperatureLimit = await GetGPUTemperatureLimitAsync().OrNullIfException().ConfigureAwait(false),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, await GetDefaultFanTableAsync().ConfigureAwait(false)),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false),
            MinValueOffset = 0,
            MaxValueOffset = 0
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    #region CPU Long Term Power Limit

    private static async Task<StepperValue> GetCPULongTermPowerLimitAsync()
    {
        var defaultValue = await WMI.LenovoCpuMethod.CPUGetDefaultPowerLimitAsync().OrNullIfException().ConfigureAwait(false);
        var (value, min, max, step) = await WMI.LenovoCpuMethod.CPUGetLongTermPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue?.longTerm);
    }

    private static Task SetCPULongTermPowerLimitAsync(int value) => WMI.LenovoCpuMethod.CPUSetLongTermPowerLimitAsync(value);

    #endregion

    #region CPU Short Term Power Limit

    private static async Task<StepperValue> GetCPUShortTermPowerLimitAsync()
    {
        var defaultValue = await WMI.LenovoCpuMethod.CPUGetDefaultPowerLimitAsync().OrNullIfException().ConfigureAwait(false);
        var (value, min, max, step) = await WMI.LenovoCpuMethod.CPUGetShortTermPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue?.shortTerm);
    }

    private static Task SetCPUShortTermPowerLimitAsync(int value) => WMI.LenovoCpuMethod.CPUSetShortTermPowerLimitAsync(value);

    #endregion

    #region CPU Peak Power Limit

    private static async Task<StepperValue> GetCPUPeakPowerLimitAsync()
    {
        var (value, min, max, step, defaultValue) = await WMI.LenovoCpuMethod.CPUGetPeakPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue);
    }

    private static Task SetCPUPeakPowerLimitAsync(int value) => WMI.LenovoCpuMethod.CPUSetPeakPowerLimitAsync(value);

    #endregion

    #region CPU Cross Loading Power Limit

    private static async Task<StepperValue> GetCPUCrossLoadingPowerLimitAsync()
    {
        var (value, min, max, step, defaultValue) = await WMI.LenovoCpuMethod.CPUGetCrossLoadingPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue);
    }

    private static Task SetCPUCrossLoadingPowerLimitAsync(int value) => WMI.LenovoCpuMethod.CPUSetCrossLoadingPowerLimitAsync(value);

    #endregion

    #region APU sPPT Power Limit

    private static async Task<StepperValue> GetAPUSPPTPowerLimitAsync()
    {
        var (value, min, max, step, defaultValue) = await WMI.LenovoCpuMethod.GetAPUSPPTPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue);
    }

    private static Task SetAPUSPPTPowerLimitAsync(int value) => WMI.LenovoCpuMethod.SetAPUSPPTPowerLimitAsync(value);

    #endregion

    #region CPU Temperature Limit

    private static async Task<StepperValue> GetCPUTemperatureLimitAsync()
    {
        var (value, min, max, step, defaultValue) = await WMI.LenovoCpuMethod.CPUGetTemperatureControlAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue);
    }

    private static Task SetCPUTemperatureLimitAsync(int value) => WMI.LenovoCpuMethod.CPUSetTemperatureControlAsync(value);

    #endregion

    #region GPU Configurable TGP

    private static async Task<StepperValue> GetGPUConfigurableTGPAsync()
    {
        var defaultValue = await WMI.LenovoGpuMethod.GPUGetDefaultPPABcTGPPowerLimit().OrNullIfException().ConfigureAwait(false);
        var (value, min, max, step) = await WMI.LenovoGpuMethod.GPUGetCTGPPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue?.ctgp);
    }

    private static Task SetGPUConfigurableTGPAsync(int value) => WMI.LenovoGpuMethod.GPUSetCTGPPowerLimitAsync(value);

    #endregion

    #region GPU Power Boost

    private static async Task<StepperValue> GetGPUPowerBoost()
    {
        var defaultValue = await WMI.LenovoGpuMethod.GPUGetDefaultPPABcTGPPowerLimit().OrNullIfException().ConfigureAwait(false);
        var (value, min, max, step) = await WMI.LenovoGpuMethod.GPUGetPPABPowerLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue?.ppab);
    }

    private static Task SetGPUPowerBoostAsync(int value) => WMI.LenovoGpuMethod.GPUSetPPABPowerLimitAsync(value);

    #endregion

    #region GPU Temperature Limit

    private static async Task<StepperValue> GetGPUTemperatureLimitAsync()
    {
        var (value, min, max, step, defaultValue) = await WMI.LenovoGpuMethod.GPUGetTemperatureLimitAsync().ConfigureAwait(false);
        return new(value, min, max, step, [], defaultValue);
    }

    private static Task SetGPUTemperatureLimitAsync(int value) => WMI.LenovoGpuMethod.GPUSetTemperatureLimitAsync(value);

    #endregion

    #region Fan Table

    private static async Task<FanTableData[]?> GetFanTableDataAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Reading fan table data...");

        var data = await WMI.LenovoFanTableData.ReadAsync().ConfigureAwait(false);

        var fanTableData = data
            .Select(d =>
            {
                var type = (d.fanId, d.sensorId) switch
                {
                    (0, 3) => FanTableType.CPU,
                    (1, 4) => FanTableType.GPU,
                    (0, 0) => FanTableType.CPUSensor,
                    _ => FanTableType.Unknown,
                };
                return new FanTableData(type, d.fanId, d.sensorId, d.fanTableData, d.sensorTableData);
            })
            .ToArray();

        if (fanTableData.Length != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table length: {fanTableData}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.FanSpeeds.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table fan speeds length: {fanTableData}");

            return null;
        }

        if (fanTableData.Count(ftd => ftd.Temps.Length == 10) != 3)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bad fan table temps length: {fanTableData}");

            return null;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Fan table data: {fanTableData}");

        return fanTableData;
    }

    private static Task SetFanTable(FanTable fanTable) => WMI.LenovoFanMethod.FanSetTableAsync(fanTable.GetBytes());

    #endregion

    #region Fan Full Speed

    private static Task<bool> GetFanFullSpeedAsync() => WMI.LenovoFanMethod.FanGetFullSpeedAsync();

    private static Task SetFanFullSpeedAsync(bool enabled) => WMI.LenovoFanMethod.FanSetFullSpeedAsync(enabled ? 1 : 0);

    #endregion

    #region Default values

    private async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultValuesInDifferentModeAsync()
    {
        var defaultFanTableAsync = await GetDefaultFanTableAsync().ConfigureAwait(false);

        var result = await WMI.LenovoDefaultValueInDifferentModeData.ReadAsync().ConfigureAwait(false);
        return result.Select(d =>
            {
                var powerMode = (PowerModeState)(d.Mode - 1);
                var defaults = new GodModeDefaults
                {
                    CPULongTermPowerLimit = d.CPULongTermPowerLimit,
                    CPUShortTermPowerLimit = d.CPUShortTermPowerLimit,
                    CPUPeakPowerLimit = d.CPUPeakPowerLimit,
                    CPUCrossLoadingPowerLimit = d.CPUCrossLoadingPowerLimit,
                    APUsPPTPowerLimit = d.APUsPPTPowerLimit,
                    CPUTemperatureLimit = d.CPUTemperatureLimit,
                    GPUPowerBoost = d.GPUPowerBoost,
                    GPUConfigurableTGP = d.GPUConfigurableTGP,
                    GPUTemperatureLimit = d.GPUTemperatureLimit,
                    FanTable = defaultFanTableAsync,
                    FanFullSpeed = false
                };
                return (powerMode, defaults);
            })
            .Where(d => d.powerMode is PowerModeState.Quiet or PowerModeState.Balance or PowerModeState.Performance)
            .DistinctBy(d => d.powerMode)
            .ToDictionary(d => d.powerMode, d => d.defaults);
    }

    #endregion

}
