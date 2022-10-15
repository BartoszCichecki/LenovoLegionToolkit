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

        public PowerModeFeature(AIModeController aiModeController)
            : base("SmartFanMode", 1, "IsSupportSmartFan")
        {
            _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
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

            await _aiModeController.StopAsync(currentState).ConfigureAwait(false);

            // Workaround: Performance mode doesn't update the dGPU temp limit (and possibly other properties) on some Gen 7 devices.
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            if (mi.Properties.HasPerformanceModeSwitchingBug && currentState == PowerModeState.Quiet && state == PowerModeState.Performance)
                await base.SetStateAsync(PowerModeState.Balance).ConfigureAwait(false);

            await base.SetStateAsync(state).ConfigureAwait(false);
        }

        public async Task EnsureCorrectPowerPlanIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(state, true).ConfigureAwait(false);
        }

        public async Task EnsureAIModeIsSetAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await _aiModeController.StartAsync(state).ConfigureAwait(false);
        }

        public async Task EnsureAIModeIsOffAsync()
        {
            var state = await GetStateAsync().ConfigureAwait(false);
            await _aiModeController.StopAsync(state).ConfigureAwait(false);
        }

        public async Task<PowerModeState> GetActualStateAsync()
        {
            var targetState = await GetStateAsync().ConfigureAwait(false);

            var currentState = await WMI.CallAsync(Scope,
                Query,
                "GetThermalMode",
                new(),
                pdc =>
                {
                    var value = Convert.ToInt32(pdc["Data"].Value) - 1;
                    if (!Enum.IsDefined(typeof(PowerModeState), value))
                        throw new InvalidOperationException($"Invalid thermal mode received. value={value}");
                    return (PowerModeState)value;
                }).ConfigureAwait(false);

            // GetThermalMode() never returns PowerModeState.GodMode so we have to check manually.
            if (currentState == PowerModeState.Performance && targetState == PowerModeState.GodMode)
                return targetState;

            return currentState;
        }
    }
}