// #define MOCK_FAN_TABLE

using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public struct GodModeState
    {
        public StepperValue CPULongTermPowerLimit { get; init; }
        public StepperValue CPUShortTermPowerLimit { get; init; }
        public StepperValue GPUPowerBoost { get; init; }
        public StepperValue GPUConfigurableTGP { get; init; }
        public FanTableInfo? FanTableInfo { get; init; }
        public bool FanFullSpeed { get; init; }
        public int MaxValueOffset { get; init; }
    }

    public class GodModeController
    {
        private readonly GodModeSettings _settings;

        public GodModeController(GodModeSettings settings)
        {
            _settings = settings;
        }

        public async Task<GodModeState> GetStateAsync()
        {
            var cpuLongTermPowerLimitState = await GetCPULongTermPowerLimitAsync().ConfigureAwait(false);
            var cpuLongTermPowerLimit = new StepperValue(_settings.Store.CPULongTermPowerLimit?.Value ?? cpuLongTermPowerLimitState.Value,
                cpuLongTermPowerLimitState.Min,
                cpuLongTermPowerLimitState.Max,
                cpuLongTermPowerLimitState.Step);

            var cpuShortTermPowerLimitState = await GetCPUShortTermPowerLimitAsync().ConfigureAwait(false);
            var cpuShortTermPowerLimit = new StepperValue(_settings.Store.CPUShortTermPowerLimit?.Value ?? cpuShortTermPowerLimitState.Value,
                cpuShortTermPowerLimitState.Min,
                cpuShortTermPowerLimitState.Max,
                cpuShortTermPowerLimitState.Step);

            var gpuPowerBoostState = await GetGPUPowerBoost().ConfigureAwait(false);
            var gpuPowerBoost = new StepperValue(_settings.Store.GPUPowerBoost?.Value ?? gpuPowerBoostState.Value,
                gpuPowerBoostState.Min,
                gpuPowerBoostState.Max,
                gpuPowerBoostState.Step);

            var gpuConfigurableTGPState = await GetGPUConfigurableTGPAsync().ConfigureAwait(false);
            var gpuConfigurableTGP = new StepperValue(_settings.Store.GPUConfigurableTGP?.Value ?? gpuConfigurableTGPState.Value,
                gpuConfigurableTGPState.Min,
                gpuConfigurableTGPState.Max,
                gpuConfigurableTGPState.Step);

            var fanTableInfo = await GetFanTableInfoAsync().ConfigureAwait(false);
            var fanFullSpeed = _settings.Store.FanFullSpeed ?? await GetFanFullSpeedAsync().ConfigureAwait(false);

            var maxValueOffset = _settings.Store.MaxValueOffset;

            return new GodModeState
            {
                CPULongTermPowerLimit = cpuLongTermPowerLimit,
                CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                GPUPowerBoost = gpuPowerBoost,
                GPUConfigurableTGP = gpuConfigurableTGP,
                FanTableInfo = fanTableInfo,
                FanFullSpeed = fanFullSpeed,
                MaxValueOffset = maxValueOffset
            };
        }

        public Task SetStateAsync(GodModeState state)
        {
            _settings.Store.CPULongTermPowerLimit = state.CPULongTermPowerLimit;
            _settings.Store.CPUShortTermPowerLimit = state.CPUShortTermPowerLimit;
            _settings.Store.GPUPowerBoost = state.GPUPowerBoost;
            _settings.Store.GPUConfigurableTGP = state.GPUConfigurableTGP;
            _settings.Store.FanTable = state.FanTableInfo?.Table;
            _settings.Store.FanFullSpeed = state.FanFullSpeed;
            _settings.Store.MaxValueOffset = state.MaxValueOffset;

            _settings.SynchronizeStore();
            return Task.CompletedTask;
        }

        public async Task ApplyStateAsync()
        {
            var cpuLongTermPowerLimit = _settings.Store.CPULongTermPowerLimit;
            var cpuShortTermPowerLimit = _settings.Store.CPUShortTermPowerLimit;
            var gpuPowerBoost = _settings.Store.GPUPowerBoost;
            var gpuConfigurableTGP = _settings.Store.GPUConfigurableTGP;
            var fanTable = _settings.Store.FanTable;
            var maxFan = _settings.Store.FanFullSpeed;

            if (cpuLongTermPowerLimit is not null)
                await SetCPULongTermPowerLimitAsync(cpuLongTermPowerLimit.Value).ConfigureAwait(false);

            if (cpuShortTermPowerLimit is not null)
                await SetCPUShortTermPowerLimitAsync(cpuShortTermPowerLimit.Value).ConfigureAwait(false);

            if (gpuPowerBoost is not null)
                await SetGPUPowerBoostAsync(gpuPowerBoost.Value).ConfigureAwait(false);

            if (gpuConfigurableTGP is not null)
                await SetGPUConfigurableTGPAsync(gpuConfigurableTGP.Value).ConfigureAwait(false);

            if (maxFan is not null)
            {
                if (fanTable is null || maxFan.Value)
                {
                    await SetFanFullSpeedAsync(maxFan.Value).ConfigureAwait(false);
                }
                else
                {
                    await SetFanFullSpeedAsync(false).ConfigureAwait(false);
                    await SetFanTable(fanTable.Value).ConfigureAwait(false);
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
            new() { { "FanTable", fanTable.ToBytes() } });

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
