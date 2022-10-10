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
        private readonly AIModeController _aiModeController;
        private readonly PowerModeListener _listener;

        public PowerModeFeature(AIModeController aiModeController, PowerModeListener listener)
            : base("SmartFanMode", 1, "IsSupportSmartFan")
        {
            _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        }

        public override async Task<PowerModeState[]> GetAllStatesAsync()
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
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

            // Workaround: Peformance mode doesn't update the dGPU temp limit (and possibly other properties) on some Gen 7 devices.
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            if (mi.Properties.HasPerformanceModeSwitchingBug && currentState == PowerModeState.Quiet && state == PowerModeState.Performance)
                await base.SetStateAsync(PowerModeState.Balance).ConfigureAwait(false);

            await base.SetStateAsync(state).ConfigureAwait(false);

            await _listener.NotifyAsync(state).ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }

        public async Task EnsureAIModeIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await _aiModeController.StartStopAsync(state).ConfigureAwait(false);
        }

        public async Task EnsureAIModeIsOffAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await _aiModeController.StopAsync(state).ConfigureAwait(false);
        }
    }
}