﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public class GodModeControllerV1 : AbstractGodModeController
{
    public GodModeControllerV1(GodModeSettings settings, Vantage vantage, LegionZone legionZone) : base(settings, vantage, legionZone) { }

    public override Task<bool> NeedsVantageDisabledAsync() => Task.FromResult(false);
    public override Task<bool> NeedsLegionZoneDisabledAsync() => Task.FromResult(true);

    public override async Task ApplyStateAsync()
    {
        if (await LegionZone.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var preset = await GetActivePresetAsync().ConfigureAwait(false);

        var cpuLongTermPowerLimit = preset.CPULongTermPowerLimit;
        var cpuShortTermPowerLimit = preset.CPUShortTermPowerLimit;
        var cpuPeakPowerLimit = preset.CPUPeakPowerLimit;
        var cpuCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit;
        var apuSPPTPowerLimit = preset.APUsPPTPowerLimit;
        var cpuTemperatureLimit = preset.CPUTemperatureLimit;
        var gpuPowerBoost = preset.GPUPowerBoost;
        var gpuConfigurableTgp = preset.GPUConfigurableTGP;
        var gpuTemperatureLimit = preset.GPUTemperatureLimit;
        var fanTable = preset.FanTable ?? FanTable.Default;
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
                throw;
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
                throw;
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

                if (!fanTable.IsValid())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    fanTable = FanTable.Default;
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

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State applied.");
    }

    public override async Task<Dictionary<PowerModeState, GodModeDefaults>> GetDefaultsInOtherPowerModesAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting defaults in other power modes...");

            var defaultValues = await GetDefaultValuesInDifferentModeAsync().ConfigureAwait(false);
            var result = defaultValues
                .Where(d => d.powerMode is PowerModeState.Quiet or PowerModeState.Balance or PowerModeState.Performance)
                .DistinctBy(d => d.powerMode)
                .ToDictionary(d => d.powerMode, d => d.defaults);

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

    public override async Task RestoreDefaultsInOtherPowerModeAsync(PowerModeState state)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Restoring defaults for {state}...");

            var defaultValues = await GetDefaultValuesInDifferentModeAsync().ConfigureAwait(false);
            var result = defaultValues
                .Where(d => d.powerMode is PowerModeState.Quiet or PowerModeState.Balance or PowerModeState.Performance)
                .DistinctBy(d => d.powerMode)
                .ToDictionary(d => d.powerMode, d => d.defaults);

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
            CPULongTermPowerLimit = await GetCPULongTermPowerLimitAsync().OrNull().ConfigureAwait(false),
            CPUShortTermPowerLimit = await GetCPUShortTermPowerLimitAsync().OrNull().ConfigureAwait(false),
            CPUPeakPowerLimit = await GetCPUPeakPowerLimitAsync().OrNull().ConfigureAwait(false),
            CPUCrossLoadingPowerLimit = await GetCPUCrossLoadingPowerLimitAsync().OrNull().ConfigureAwait(false),
            APUsPPTPowerLimit = await GetAPUSPPTPowerLimitAsync().OrNull().ConfigureAwait(false),
            CPUTemperatureLimit = await GetCPUTemperatureLimitAsync().OrNull().ConfigureAwait(false),
            GPUPowerBoost = await GetGPUPowerBoost().OrNull().ConfigureAwait(false),
            GPUConfigurableTGP = await GetGPUConfigurableTGPAsync().OrNull().ConfigureAwait(false),
            GPUTemperatureLimit = await GetGPUTemperatureLimitAsync().OrNull().ConfigureAwait(false),
            FanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default),
            FanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false),
            MaxValueOffset = 0
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Default state retrieved: {preset}");

        return preset;
    }

    #region CPU Long Term Power Limit

    private static async Task<StepperValue> GetCPULongTermPowerLimitAsync()
    {
        var defaultValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Default_PowerLimit",
            new(),
            pdc => Convert.ToInt32(pdc["DefaultLongTermPowerlimit"].Value)).OrNull().ConfigureAwait(false);

        var stepperValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_LongTerm_PowerLimit",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentLongTerm_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinLongTerm_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxLongTerm_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
            }).ConfigureAwait(false);

        return stepperValue;
    }

    private static Task SetCPULongTermPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_LongTerm_PowerLimit",
        new() { { "value", $"{value}" } });

    #endregion

    #region CPU Short Term Power Limit

    private static async Task<StepperValue> GetCPUShortTermPowerLimitAsync()
    {
        var defaultValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Default_PowerLimit",
            new(),
            pdc => Convert.ToInt32(pdc["DefaultShortTermPowerlimit"].Value)).OrNull().ConfigureAwait(false);

        var stepperValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_ShortTerm_PowerLimit",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentShortTerm_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinShortTerm_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxShortTerm_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
            }).ConfigureAwait(false);

        return stepperValue;
    }

    private static Task SetCPUShortTermPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_ShortTerm_PowerLimit",
        new() { { "value", $"{value}" } });

    #endregion

    #region CPU Peak Power Limit

    private static Task<StepperValue> GetCPUPeakPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_Peak_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentPeakPowerLimit"].Value);
            var min = Convert.ToInt32(pdc["MinPeakPowerLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxPeakPowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultPeakPowerLimit"].Value);

            return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
        });

    private static Task SetCPUPeakPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_Peak_PowerLimit",
        new() { { "CurrentPeakPowerLimit", $"{value}" } });

    #endregion

    #region CPU Cross Loading Power Limit

    private static Task<StepperValue> GetCPUCrossLoadingPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_Cross_Loading_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentCpuCrossLoading"].Value);
            var min = Convert.ToInt32(pdc["MinCpuCrossLoading"].Value);
            var max = Convert.ToInt32(pdc["MaxCpuCrossLoading"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultCpuCrossLoading"].Value);

            return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
        });

    private static Task SetCPUCrossLoadingPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_Cross_Loading_PowerLimit",
        new() { { "CurrentCpuCrossLoading", $"{value}" } });

    #endregion

    #region APU sPPT Power Limit

    private static Task<StepperValue> GetAPUSPPTPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "Get_APU_sPPT_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrenAPUsPPTPowerLimit"].Value);
            var min = Convert.ToInt32(pdc["MinAPUsPPTPowerLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxAPUsPPTPowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultAPUsPPTPowerLimit"].Value);

            return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
        });

    private static Task SetAPUSPPTPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "Set_APU_sPPT_PowerLimit",
        new() { { "CurrentAPUsPPTPowerLimit", $"{value}" } });

    #endregion

    #region CPU Temperature Limit

    private static Task<StepperValue> GetCPUTemperatureLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_Temperature_Control",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentTemperatueControl"].Value);
            var min = Convert.ToInt32(pdc["MinTemperatueControl"].Value);
            var max = Convert.ToInt32(pdc["MaxTemperatueControl"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultTemperatueControl"].Value);

            return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
        });

    private static Task SetCPUTemperatureLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_Temperature_Control",
        new() { { "CurrentTemperatureControl", $"{value}" } });

    #endregion

    #region GPU Configurable TGP

    private static async Task<StepperValue> GetGPUConfigurableTGPAsync()
    {
        var defaultValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_Default_PPAB_cTGP_PowerLimit",
            new(),
            pdc => Convert.ToInt32(pdc["Default_cTGP_Powerlimit"].Value)).OrNull().ConfigureAwait(false);

        var stepperValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_cTGP_PowerLimit",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Current_cTGP_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["Min_cTGP_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["Max_cTGP_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
            }).ConfigureAwait(false);

        return stepperValue;
    }

    private static Task SetGPUConfigurableTGPAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_cTGP_PowerLimit",
        new() { { "value", $"{value}" } });

    #endregion

    #region GPU Power Boost

    private static async Task<StepperValue> GetGPUPowerBoost()
    {
        var defaultValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_Default_PPAB_cTGP_PowerLimit",
            new(),
            pdc => Convert.ToInt32(pdc["Default_PPAB_Powerlimit"].Value)).OrNull().ConfigureAwait(false);

        var stepperValue = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_PPAB_PowerLimit",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentPPAB_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinPPAB_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxPPAB_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
            }).ConfigureAwait(false);

        return stepperValue;
    }

    private static Task SetGPUPowerBoostAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_PPAB_PowerLimit",
        new() { { "value", $"{value}" } });

    #endregion

    #region GPU Temperature Limit

    private static Task<StepperValue> GetGPUTemperatureLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Get_Temperature_Limit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentTemperatueLimit"].Value);
            var min = Convert.ToInt32(pdc["MinTemperatueLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxTemperatueLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);
            var defaultValue = Convert.ToInt32(pdc["DefaultTemperatueLimit"].Value);

            return new StepperValue(value, min, max, step, Array.Empty<int>(), defaultValue);
        });

    private static Task SetGPUTemperatureLimitAsync(int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_Temperature_Limit",
        new() { { "CurrentTemperatureLimit", $"{value}" } });

    #endregion

    #region Fan Table

    protected static async Task<FanTableData[]?> GetFanTableDataAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Reading fan table data...");

        var data = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA",
            pdc =>
            {
                var fanId = Convert.ToByte(pdc["Fan_Id"].Value);
                var sensorId = Convert.ToByte(pdc["Sensor_ID"].Value);
                var fanSpeeds = (ushort[]?)pdc["FanTable_Data"].Value ?? Array.Empty<ushort>();
                var temps = (ushort[]?)pdc["SensorTable_Data"].Value ?? Array.Empty<ushort>();

                var type = (fanId, sensorId) switch
                {
                    (0, 3) => FanTableType.CPU,
                    (1, 4) => FanTableType.GPU,
                    (0, 0) => FanTableType.CPUSensor,
                    _ => FanTableType.Unknown,
                };

                return new FanTableData
                {
                    Type = type,
                    FanId = fanId,
                    SensorId = sensorId,
                    FanSpeeds = fanSpeeds,
                    Temps = temps
                };
            }).ConfigureAwait(false);

        var fanTableData = data.ToArray();

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

    protected static Task SetFanTable(FanTable fanTable) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_Table",
        new() { { "FanTable", fanTable.GetBytes() } });

    #endregion

    #region Fan Full Speed

    protected static Task<bool> GetFanFullSpeedAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Get_FullSpeed",
        new(),
        pdc => (bool)pdc["Status"].Value);

    protected static Task SetFanFullSpeedAsync(bool enabled) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_FullSpeed",
        new() { { "Status", enabled } });

    #endregion

    #region Default values

    private static Task<IEnumerable<(PowerModeState powerMode, GodModeDefaults defaults)>> GetDefaultValuesInDifferentModeAsync() => WMI.ReadAsync("root\\WMI",
        $"SELECT * FROM LENOVO_DEFAULT_VALUE_IN_DIFFERENT_MODE_DATA ",
        pdc =>
        {
            var mode = Convert.ToInt32(pdc["mode"].Value) - 1;

            return ((PowerModeState)mode, new GodModeDefaults
            {
                CPULongTermPowerLimit = Convert.ToInt32(pdc["DefaultLongTermPowerlimit"].Value),
                CPUShortTermPowerLimit = Convert.ToInt32(pdc["DefaultShortTermPowerlimit"].Value),
                CPUPeakPowerLimit = Convert.ToInt32(pdc["DefaultPeakPowerLimit"].Value),
                CPUCrossLoadingPowerLimit = Convert.ToInt32(pdc["DefaultCpuCrossLoading"].Value),
                APUsPPTPowerLimit = Convert.ToInt32(pdc["DefaultAPUsPPTPowerLimit"].Value),
                CPUTemperatureLimit = Convert.ToInt32(pdc["DefaultTemperatueControl"].Value),
                GPUPowerBoost = Convert.ToInt32(pdc["Default_PPAB_Powerlimit"].Value),
                GPUConfigurableTGP = Convert.ToInt32(pdc["Default_cTGP_Powerlimit"].Value),
                GPUTemperatureLimit = Convert.ToInt32(pdc["DefaultTemperatueLimit"].Value),
                FanTable = FanTable.Default,
                FanFullSpeed = false
            });
        });

    #endregion

}