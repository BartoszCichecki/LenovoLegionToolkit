using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class PowerModeFeature : AbstractLenovoGamezoneWmiFeature<PowerModeState>
    {
        private readonly GodModeController _controller;
        private readonly PowerModeListener _listener;

        public PowerModeFeature(GodModeController controller, PowerModeListener listener) : base("SmartFanMode", 1, "IsSupportSmartFan")
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        }

        public override async Task<PowerModeState[]> GetAllStatesAsync()
        {
            var mi = await Compatibility.GetMachineInformation().ConfigureAwait(false);
            if (mi.Properties.SupportsGodMode)
                return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance, PowerModeState.GodMode };

            return new[] { PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance };
        }

        public override async Task SetStateAsync(PowerModeState state)
        {
            var allStates = await GetAllStatesAsync().ConfigureAwait(false);
            if (!allStates.Contains(state))
                throw new InvalidOperationException($"Unsupported power mode {state}.");

            var currentState = await GetStateAsync().ConfigureAwait(false);

            await base.SetStateAsync(state).ConfigureAwait(false);

            if (state == PowerModeState.GodMode)
                await _controller.ApplyStateAsync().ConfigureAwait(false);

            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);

            if (state != currentState)
                await _listener.NotifyAsync(state).ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }
    }
}