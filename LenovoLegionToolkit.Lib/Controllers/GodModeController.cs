using System;
using System.Threading.Tasks;
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

            var fanFullSpeed = _settings.Store.FanFullSpeed ?? await GetFanFullSpeedAsync().ConfigureAwait(false);

            var maxValueOffset = _settings.Store.MaxValueOffset;

            return new GodModeState
            {
                CPULongTermPowerLimit = cpuLongTermPowerLimit,
                CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                GPUPowerBoost = gpuPowerBoost,
                GPUConfigurableTGP = gpuConfigurableTGP,
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
            var maxFan = _settings.Store.FanFullSpeed;

            if (cpuLongTermPowerLimit is null) return;
            if (cpuShortTermPowerLimit is null) return;
            if (gpuPowerBoost is null) return;
            if (gpuConfigurableTGP is null) return;
            if (maxFan is null) return;

            await SetCPULongTermPowerLimitAsync(cpuLongTermPowerLimit.Value).ConfigureAwait(false);
            await SetCPUShortTermPowerLimitAsync(cpuShortTermPowerLimit.Value).ConfigureAwait(false);
            await SetGPUPowerBoostAsync(gpuPowerBoost.Value).ConfigureAwait(false);
            await SetGPUConfigurableTGPAsync(gpuConfigurableTGP.Value).ConfigureAwait(false);
            await SetFanFullSpeedAsync(maxFan.Value).ConfigureAwait(false);
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
