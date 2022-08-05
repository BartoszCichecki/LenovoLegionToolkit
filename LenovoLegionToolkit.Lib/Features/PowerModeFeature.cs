using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractLenovoGamezoneWmiFeature<PowerModeState>
    {
        private readonly GodModeController _controller;

        public PowerModeFeature(GodModeController controller) : base("SmartFanMode", 1, "IsSupportSmartFan")
        {
            _controller = controller;
        }

        public override async Task<PowerModeState[]> GetAllStatesAsync()
        {
            var mi = await Compatibility.GetMachineInformation().ConfigureAwait(false);
            if (mi.Properties.SupportsGodMode)
                return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance, PowerModeState.GodMode };
            else
                return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance };
        }

        public override async Task SetStateAsync(PowerModeState state)
        {
            await base.SetStateAsync(state).ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);

            if (state == PowerModeState.GodMode)
                await _controller.ApplyStateAsync().ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }
    }
}