using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class PowerModeAutomationStep : IAutomationStep
    {
        private readonly PowerModeFeature _feature = DIContainer.Resolve<PowerModeFeature>();
        private readonly PowerModeState _state;

        public PowerModeAutomationStep(PowerModeState state)
        {
            _state = state;
        }

        public async Task RunAsync()
        {
            var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
            if (_state == currentState)
                return;
            await _feature.SetStateAsync(_state).ConfigureAwait(false);
        }

        public IAutomationStep DeepCopy() => new PowerModeAutomationStep(_state);
    }
}
