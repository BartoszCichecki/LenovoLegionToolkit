using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        private readonly FanCoolingFeature _fanCoolingFeature;
        private readonly GodModeSettings _godModeSettings;

        public PowerModeFeature(FanCoolingFeature fanCoolingFeature, GodModeSettings godModeSettings) : base("SmartFanMode", 1, "IsSupportSmartFan")
        {
            _fanCoolingFeature = fanCoolingFeature;
            _godModeSettings = godModeSettings;
        }

        public override async Task<PowerModeState[]> GetAllStatesAsync()
        {
            var mi = await Compatibility.GetMachineInformation().ConfigureAwait(false);
            if (mi.ModelYear == ModelYear.MY2021OrLater)
                return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance, PowerModeState.GodMode };
            else
                return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance };
        }

        public override async Task SetStateAsync(PowerModeState state)
        {
            await base.SetStateAsync(state).ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);

            if (state == PowerModeState.GodMode)
                await ApplyGodModeSettingsAsync().ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }

        private async Task ApplyGodModeSettingsAsync()
        {
            var fanCooling = _godModeSettings.Store.FanCooling ?? false;
            await _fanCoolingFeature.SetStateAsync(fanCooling ? FanCoolingState.On : FanCoolingState.Off).ConfigureAwait(false);
        }
    }
}