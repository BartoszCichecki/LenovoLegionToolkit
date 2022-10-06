// #define MOCK_FAN_TABLE

using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public struct GodModeState
    {
        public StepperValue? CPULongTermPowerLimit { get; init; }
        public StepperValue? CPUShortTermPowerLimit { get; init; }
        public StepperValue? CPUCrossLoadingPowerLimit { get; init; }
        public StepperValue? CPUTemperatureLimit { get; init; }
        public StepperValue? GPUPowerBoost { get; init; }
        public StepperValue? GPUConfigurableTGP { get; init; }
        public StepperValue? GPUTemperatureLimit { get; init; }
        public FanTableInfo? FanTableInfo { get; init; }
        public bool FanFullSpeed { get; init; }
        public int MaxValueOffset { get; init; }
    }

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
            var cpuLongTermPowerLimitState = await GetCPULongTermPowerLimitAsync().OrNull().ConfigureAwait(false);
            var cpuLongTermPowerLimit = CreateStepperValue(cpuLongTermPowerLimitState, _settings.Store.CPULongTermPowerLimit);

            var cpuShortTermPowerLimitState = await GetCPUShortTermPowerLimitAsync().OrNull().ConfigureAwait(false);
            var cpuShortTermPowerLimit = CreateStepperValue(cpuShortTermPowerLimitState, _settings.Store.CPUShortTermPowerLimit);

            var cpuCrossLoadingPowerLimitState = await GetCPUCrossLoadingPowerLimitAsync().OrNull().ConfigureAwait(false);
            var cpuCrossLoadingPowerLimit = CreateStepperValue(cpuCrossLoadingPowerLimitState, _settings.Store.CPUCrossLoadingPowerLimit);

            var cpuTemperatureLimitState = await GetCPUTemperatureLimitAsync().OrNull().ConfigureAwait(false);
            var cpuTemperatureLimit = CreateStepperValue(cpuTemperatureLimitState, _settings.Store.CPUTemperatureLimit);

            var gpuPowerBoostState = await GetGPUPowerBoost().OrNull().ConfigureAwait(false);
            var gpuPowerBoost = CreateStepperValue(gpuPowerBoostState, _settings.Store.GPUPowerBoost);

            var gpuConfigurableTGPState = await GetGPUConfigurableTGPAsync().OrNull().ConfigureAwait(false);
            var gpuConfigurableTGP = CreateStepperValue(gpuConfigurableTGPState, _settings.Store.GPUConfigurableTGP);

            var gpuTemperatureLimitState = await GetGPUTemperatureLimitAsync().OrNull().ConfigureAwait(false);
            var gpuTemperatureLimit = CreateStepperValue(gpuTemperatureLimitState, _settings.Store.GPUTemperatureLimit);

            var fanTableInfo = await GetFanTableInfoAsync().ConfigureAwait(false);
            var fanFullSpeed = _settings.Store.FanFullSpeed ?? await GetFanFullSpeedAsync().ConfigureAwait(false);

            var maxValueOffset = _settings.Store.MaxValueOffset;

            return new GodModeState
            {
                CPULongTermPowerLimit = cpuLongTermPowerLimit,
                CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                CPUCrossLoadingPowerLimit = cpuCrossLoadingPowerLimit,
                CPUTemperatureLimit = cpuTemperatureLimit,
                GPUPowerBoost = gpuPowerBoost,
                GPUConfigurableTGP = gpuConfigurableTGP,
                GPUTemperatureLimit = gpuTemperatureLimit,
                FanTableInfo = fanTableInfo,
                FanFullSpeed = fanFullSpeed,
                MaxValueOffset = maxValueOffset
            };
        }

        public Task SetStateAsync(GodModeState state)
        {
            _settings.Store.CPULongTermPowerLimit = state.CPULongTermPowerLimit;
            _settings.Store.CPUShortTermPowerLimit = state.CPUShortTermPowerLimit;
            _settings.Store.CPUCrossLoadingPowerLimit = state.CPUCrossLoadingPowerLimit;
            _settings.Store.CPUTemperatureLimit = state.CPUTemperatureLimit;
            _settings.Store.GPUPowerBoost = state.GPUPowerBoost;
            _settings.Store.GPUConfigurableTGP = state.GPUConfigurableTGP;
            _settings.Store.GPUTemperatureLimit = state.GPUTemperatureLimit;
            _settings.Store.FanTable = state.FanTableInfo?.Table;
            _settings.Store.FanFullSpeed = state.FanFullSpeed;
            _settings.Store.MaxValueOffset = state.MaxValueOffset;

            _settings.SynchronizeStore();
            return Task.CompletedTask;
        }

        public async Task ApplyStateAsync()
        {
            if (await _legionZone.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't correctly apply state when Legion Zone is running.");

            var cpuLongTermPowerLimit = _settings.Store.CPULongTermPowerLimit;
            var cpuShortTermPowerLimit = _settings.Store.CPUShortTermPowerLimit;
            var cpuCrossLoadingPowerLimit = _settings.Store.CPUCrossLoadingPowerLimit;
            var cpuTemperatureLimit = _settings.Store.CPUTemperatureLimit;
            var gpuPowerBoost = _settings.Store.GPUPowerBoost;
            var gpuConfigurableTGP = _settings.Store.GPUConfigurableTGP;
            var gpuTemperatureLimit = _settings.Store.GPUTemperatureLimit;
            var fanTable = _settings.Store.FanTable;
            var maxFan = _settings.Store.FanFullSpeed;

            if (cpuLongTermPowerLimit is not null)
            {
                try
                {
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
                    await SetGPUPowerBoostAsync(gpuPowerBoost.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=gpuPowerBoost]", ex);
                    throw;
                }
            }

            if (gpuConfigurableTGP is not null)
            {
                try
                {
                    await SetGPUConfigurableTGPAsync(gpuConfigurableTGP.Value).ConfigureAwait(false);
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
                    await SetGPUTemperatureLimitAsync(gpuTemperatureLimit.Value).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Apply failed. [setting=gpuTemperatureLimit]", ex);
                    throw;
                }
            }

            if (maxFan is not null)
            {
                if (fanTable is null || maxFan.Value)
                {
                    try
                    {
                        await SetFanFullSpeedAsync(maxFan.Value).ConfigureAwait(false);
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
                        await SetFanTable(fanTable.Value).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Apply failed. [setting=fanTable]", ex);
                        throw;
                    }
                }
            }
        }

        private async Task<FanTableInfo?> GetFanTableInfoAsync()
        {
            var fanTableData = await GetFanTableDataAsync().ConfigureAwait(false);
            if (fanTableData is null)
                return null;

            var fanTable = _settings.Store.FanTable ?? GetDefaultFanTable();
            return new FanTableInfo(fanTableData, fanTable);
        }

        private static StepperValue? CreateStepperValue(StepperValue? state, StepperValue? store)
        {
            if (!state.HasValue || state.Value.Min == state.Value.Max)
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

        private static FanTable GetDefaultFanTable() => new(new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

        private async Task<FanTableData[]?> GetFanTableDataAsync()
        {
            var data = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA",
                pdc =>
                {
                    var fanId = Convert.ToByte(pdc["Fan_Id"].Value);
                    var sensorId = Convert.ToByte(pdc["Sensor_ID"].Value);
                    var fanSpeeds = (ushort[])pdc["FanTable_Data"].Value ?? Array.Empty<ushort>();
                    var temps = (ushort[])pdc["SensorTable_Data"].Value ?? Array.Empty<ushort>();
                    return new FanTableData
                    {
                        FanId = fanId,
                        SensorId = sensorId,
                        FanSpeeds = fanSpeeds,
                        Temps = temps
                    };
                }).ConfigureAwait(false);

#if !MOCK_FAN_TABLE
            var fanTableData = data.ToArray();
#else
            var fanTableData = new FanTableData[]
            {
                new()
                {
                    FanId = 0,
                    SensorId = 3,
                    FanSpeeds = new ushort[] { 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000 },
                    Temps = new ushort[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }
                },
                new()
                {
                    FanId = 1,
                    SensorId = 4,
                    FanSpeeds = new ushort[] { 1100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000 },
                    Temps = new ushort[] { 20, 11, 12, 13, 14, 15, 16, 17, 18, 19 }
                },
                new()
                {
                    FanId = 0,
                    SensorId = 0,
                    FanSpeeds = new ushort[] { 1000, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000 },
                    Temps = new ushort[] { 30, 11, 12, 13, 14, 15, 16, 17, 18, 19 }
                }
            };
#endif

            if (fanTableData.Length != 3)
                return null;

            if (fanTableData.Count(ftd => ftd.FanSpeeds.Length == 10) != 3)
                return null;

            if (fanTableData.Count(ftd => ftd.Temps.Length == 10) != 3)
                return null;

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
}
