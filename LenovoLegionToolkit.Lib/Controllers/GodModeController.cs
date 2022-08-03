using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public struct GodModeState
    {
        public StepperValue GPUPowerBoost { get; }
        public bool FanCooling { get; }

        public GodModeState(StepperValue gpuPowerBoost, bool fanCooling)
        {
            GPUPowerBoost = gpuPowerBoost;
            FanCooling = fanCooling;
        }
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
            var gpuPowerBoostState = await GetGPUPowerBoost().ConfigureAwait(false);
            var gpuPowerBoost = new StepperValue(_settings.Store.GPUPowerBoost?.Value ?? gpuPowerBoostState.Value,
                gpuPowerBoostState.Min,
                gpuPowerBoostState.Max,
                gpuPowerBoostState.Step);

            var fanCooling = _settings.Store.FanCooling ?? await GetFanCoolingAsync().ConfigureAwait(false);

            return new(gpuPowerBoost, fanCooling);
        }

        public Task SetStateAsync(GodModeState state)
        {
            _settings.Store.GPUPowerBoost = state.GPUPowerBoost;
            _settings.Store.FanCooling = state.FanCooling;

            _settings.SynchronizeStore();
            return Task.CompletedTask;
        }

        public async Task ApplyStateAsync()
        {
            var gpuPowerBoost = _settings.Store.GPUPowerBoost;
            var maxFan = _settings.Store.FanCooling;

            if (gpuPowerBoost is null) return;
            if (maxFan is null) return;

            await SetGPUPowerBoostAsync(gpuPowerBoost.Value).ConfigureAwait(false);
            await SetFanCoolingAsync(maxFan.Value).ConfigureAwait(false);
        }

        #region GPU cTGP

        private Task<StepperValue> GetGPUcTGPAsync() => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_cTGP_PowerLimit",
            new Dictionary<string, object>(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Current_cTGP_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["Min_cTGP_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["Max_cTGP_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return new StepperValue(value, min, max, step);
            });

        private Task SetGPUcTGPAsync(StepperValue value) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Set_cTGP_PowerLimit",
            new Dictionary<string, object>() { { "value", $"{value.Value}" } });

        #endregion

        #region GPU Power Boost

        private Task<StepperValue> GetGPUPowerBoost() => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_PPAB_PowerLimit",
            new Dictionary<string, object>(),
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
            new Dictionary<string, object>() { { "value", $"{value.Value}" } });

        #endregion

        #region Fan cooling

        private Task<bool> GetFanCoolingAsync() => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetFanCoolingStatus",
            new Dictionary<string, object>(),
            pdc =>
            {
                var data = (uint)pdc["Data"].Value;
                return data > 0;
            });

        private Task SetFanCoolingAsync(bool enabled) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetFanCooling",
            new Dictionary<string, object>() { { "Data", enabled ? "1" : "0" } });

        #endregion

    }
}
