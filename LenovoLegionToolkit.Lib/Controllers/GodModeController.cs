using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers;

public class GodModeController
{
    private readonly GodModeSettings _settings;
    private readonly LegionZone _legionZone;

    public GodModeController(GodModeSettings settings, LegionZone legionZone)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _legionZone = legionZone ?? throw new ArgumentNullException(nameof(legionZone));
    }

    public async Task<GodModeState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var store = _settings.Store;
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
                CPUCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit,
                CPUTemperatureLimit = preset.CPUTemperatureLimit,
                GPUPowerBoost = preset.GPUPowerBoost,
                GPUConfigurableTGP = preset.GPUConfigurableTGP,
                GPUTemperatureLimit = preset.GPUTemperatureLimit,
                FanTable = preset.FanTableInfo?.Table,
                FanFullSpeed = preset.FanFullSpeed,
                MaxValueOffset = preset.MaxValueOffset,
            });
        }

        _settings.Store.ActivePresetId = activePresetId;
        _settings.Store.Presets = presets;
        _settings.SynchronizeStore();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State saved.");

        return Task.CompletedTask;
    }

    public async Task ApplyStateAsync()
    {
        if (await _legionZone.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            throw new InvalidOperationException("Can't correctly apply state when Legion Zone is running.");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying state...");

        var activePresetId = _settings.Store.ActivePresetId;
        var presets = _settings.Store.Presets;

        if (!presets.ContainsKey(activePresetId))
            throw new InvalidOperationException($"Preset with ID {activePresetId} not found.");

        var preset = presets[activePresetId];

        var cpuLongTermPowerLimit = preset.CPULongTermPowerLimit;
        var cpuShortTermPowerLimit = preset.CPUShortTermPowerLimit;
        var cpuCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit;
        var cpuTemperatureLimit = preset.CPUTemperatureLimit;
        var gpuPowerBoost = preset.GPUPowerBoost;
        var gpuConfigurableTgp = preset.GPUConfigurableTGP;
        var gpuTemperatureLimit = preset.GPUTemperatureLimit;
        var fanTable = preset.FanTable;
        var maxFan = preset.FanFullSpeed;

        if (cpuLongTermPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Long Term Power Limit: {cpuLongTermPowerLimit}");

                await SetCPULongTermPowerLimitAsync(cpuLongTermPowerLimit.Value).ConfigureAwait(false);
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

                await SetCPUShortTermPowerLimitAsync(cpuShortTermPowerLimit.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuShortTermPowerLimit]", ex);
                throw;
            }
        }

        if (cpuCrossLoadingPowerLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Cross Loading Power Limit: {cpuCrossLoadingPowerLimit}");

                await SetCPUCrossLoadingPowerLimitAsync(cpuCrossLoadingPowerLimit.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=cpuCrossLoadingPowerLimit]", ex);
                throw;
            }
        }

        if (cpuTemperatureLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying CPU Temperature Limit: {cpuTemperatureLimit}");

                await SetCPUTemperatureLimitAsync(cpuTemperatureLimit.Value).ConfigureAwait(false);
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

                await SetGPUPowerBoostAsync(gpuPowerBoost.Value).ConfigureAwait(false);
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

                await SetGPUConfigurableTGPAsync(gpuConfigurableTgp.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=gpuConfigurableTGP]", ex);
                throw;
            }
        }

        if (gpuTemperatureLimit is not null)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying GPU Temperature Limit: {gpuTemperatureLimit}");

                await SetGPUTemperatureLimitAsync(gpuTemperatureLimit.Value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=gpuTemperatureLimit]", ex);
                throw;
            }
        }

        if (fanTable is null || maxFan)
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Full speed: {maxFan}");

                await SetFanFullSpeedAsync(maxFan).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=maxFan]", ex);
                throw;
            }
        }
        else
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Making sure Fan Full Speed is off...");

                await SetFanFullSpeedAsync(false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Apply failed. [setting=maxFan]", ex);
                throw;
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Applying Fan Table {fanTable.Value}");

                var table = fanTable.Value;

                if (!fanTable.Value.IsValid())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Fan table invalid, replacing with default...");

                    table = FanTable.Default;
                }

                await SetFanTable(table).ConfigureAwait(false);
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

    private async Task<GodModePreset> GetDefaultStateAsync()
    {
        var cpuLongTermPowerLimit = await GetCPULongTermPowerLimitAsync().OrNull().ConfigureAwait(false);
        var cpuShortTermPowerLimit = await GetCPUShortTermPowerLimitAsync().OrNull().ConfigureAwait(false);
        var cpuCrossLoadingPowerLimit = await GetCPUCrossLoadingPowerLimitAsync().OrNull().ConfigureAwait(false);
        var cpuTemperatureLimit = await GetCPUTemperatureLimitAsync().OrNull().ConfigureAwait(false);
        var gpuPowerBoost = await GetGPUPowerBoost().OrNull().ConfigureAwait(false);
        var gpuConfigurableTgp = await GetGPUConfigurableTGPAsync().OrNull().ConfigureAwait(false);
        var gpuTemperatureLimit = await GetGPUTemperatureLimitAsync().OrNull().ConfigureAwait(false);
        var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);
        FanTableInfo? fanTableInfo = fanTableData is null ? null : new FanTableInfo(fanTableData, FanTable.Default);
        var fanFullSpeed = await GetFanFullSpeedAsync().ConfigureAwait(false);

        return new GodModePreset
        {
            Name = "Default",
            CPULongTermPowerLimit = cpuLongTermPowerLimit,
            CPUShortTermPowerLimit = cpuShortTermPowerLimit,
            CPUCrossLoadingPowerLimit = cpuCrossLoadingPowerLimit,
            CPUTemperatureLimit = cpuTemperatureLimit,
            GPUPowerBoost = gpuPowerBoost,
            GPUConfigurableTGP = gpuConfigurableTgp,
            GPUTemperatureLimit = gpuTemperatureLimit,
            FanTableInfo = fanTableInfo,
            FanFullSpeed = fanFullSpeed,
            MaxValueOffset = 0
        };
    }

    private bool IsValidStore(GodModeSettings.GodModeSettingsStore store) => store.Presets.Any() && store.Presets.ContainsKey(store.ActivePresetId);

    private GodModeState LoadStateFromStore(GodModeSettings.GodModeSettingsStore store, GodModePreset defaultState)
    {
        var states = new Dictionary<Guid, GodModePreset>();

        foreach (var (id, preset) in store.Presets)
        {
            states.Add(id, new GodModePreset
            {
                Name = preset.Name,
                CPULongTermPowerLimit = CreateStepperValue(defaultState.CPULongTermPowerLimit, preset.CPULongTermPowerLimit, preset.MaxValueOffset),
                CPUShortTermPowerLimit = CreateStepperValue(defaultState.CPUShortTermPowerLimit, preset.CPUShortTermPowerLimit, preset.MaxValueOffset),
                CPUCrossLoadingPowerLimit = CreateStepperValue(defaultState.CPUCrossLoadingPowerLimit, preset.CPUCrossLoadingPowerLimit, preset.MaxValueOffset),
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

    private static StepperValue? CreateStepperValue(StepperValue? state, StepperValue? store = null, int maxValueOffset = 0)
    {
        if (!state.HasValue || state.Value.Min == state.Value.Max + maxValueOffset)
            return null;

        return new StepperValue(store?.Value ?? state.Value.Value, state.Value.Min, state.Value.Max, state.Value.Step);
    }

    #region CPU Long Term Power Limit

    private Task<StepperValue> GetCPULongTermPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_LongTerm_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentLongTerm_PowerLimit"].Value);
            var min = Convert.ToInt32(pdc["MinLongTerm_PowerLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxLongTerm_PowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetCPULongTermPowerLimitAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_LongTerm_PowerLimit",
        new() { { "value", $"{value.Value}" } });

    #endregion

    #region CPU Short Term Power Limit

    private Task<StepperValue> GetCPUShortTermPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_ShortTerm_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentShortTerm_PowerLimit"].Value);
            var min = Convert.ToInt32(pdc["MinShortTerm_PowerLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxShortTerm_PowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetCPUShortTermPowerLimitAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_ShortTerm_PowerLimit",
        new() { { "value", $"{value.Value}" } });

    #endregion

    #region CPU Cross Loading Power Limit

    private Task<StepperValue> GetCPUCrossLoadingPowerLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_Cross_Loading_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentCpuCrossLoading"].Value);
            var min = Convert.ToInt32(pdc["MinCpuCrossLoading"].Value);
            var max = Convert.ToInt32(pdc["MaxCpuCrossLoading"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetCPUCrossLoadingPowerLimitAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_Cross_Loading_PowerLimit",
        new() { { "CurrentCpuCrossLoading", $"{value.Value}" } });

    #endregion

    #region CPU Temperature Limit

    private Task<StepperValue> GetCPUTemperatureLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Get_Temperature_Control",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentTemperatueControl"].Value);
            var min = Convert.ToInt32(pdc["MinTemperatueControl"].Value);
            var max = Convert.ToInt32(pdc["MaxTemperatueControl"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetCPUTemperatureLimitAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_CPU_METHOD",
        "CPU_Set_Temperature_Control",
        new() { { "CurrentTemperatureControl", $"{value.Value}" } });

    #endregion

    #region GPU Configurable TGP

    private Task<StepperValue> GetGPUConfigurableTGPAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Get_cTGP_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["Current_cTGP_PowerLimit"].Value);
            var min = Convert.ToInt32(pdc["Min_cTGP_PowerLimit"].Value);
            var max = Convert.ToInt32(pdc["Max_cTGP_PowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetGPUConfigurableTGPAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_cTGP_PowerLimit",
        new() { { "value", $"{value.Value}" } });

    #endregion

    #region GPU Power Boost

    private Task<StepperValue> GetGPUPowerBoost() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Get_PPAB_PowerLimit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentPPAB_PowerLimit"].Value);
            var min = Convert.ToInt32(pdc["MinPPAB_PowerLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxPPAB_PowerLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetGPUPowerBoostAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_PPAB_PowerLimit",
        new() { { "value", $"{value.Value}" } });

    #endregion

    #region GPU Temperature Limit

    private Task<StepperValue> GetGPUTemperatureLimitAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Get_Temperature_Limit",
        new(),
        pdc =>
        {
            var value = Convert.ToInt32(pdc["CurrentTemperatueLimit"].Value);
            var min = Convert.ToInt32(pdc["MinTemperatueLimit"].Value);
            var max = Convert.ToInt32(pdc["MaxTemperatueLimit"].Value);
            var step = Convert.ToInt32(pdc["step"].Value);

            return new StepperValue(value, min, max, step);
        });

    private Task SetGPUTemperatureLimitAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_GPU_METHOD",
        "GPU_Set_Temperature_Limit",
        new() { { "CurrentTemperatureLimit", $"{value.Value}" } });

    #endregion

    #region Fan Table

    private async Task<FanTableData[]?> GetFanTableDataAsync()
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

                return new FanTableData
                {
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

    private Task SetFanTable(FanTable fanTable) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_Table",
        new() { { "FanTable", fanTable.GetBytes() } });

    #endregion

    #region Fan Full Speed

    private Task<bool> GetFanFullSpeedAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Get_FullSpeed",
        new(),
        pdc =>
        {
            return (bool)pdc["Status"].Value;
        });

    private Task SetFanFullSpeedAsync(bool enabled) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_Set_FullSpeed",
        new() { { "Status", enabled } });

    #endregion

}