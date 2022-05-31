using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class PowerModeAutomationStep : IAutomationStep<PowerModeState>
    {
        private readonly PowerModeFeature _feature = DIContainer.Resolve<PowerModeFeature>();

        public PowerModeState State { get; }

        public PowerModeAutomationStep(PowerModeState state)
        {
            State = state;
        }

        public async Task RunAsync()
        {
            var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
            if (State == currentState)
                return;
            await _feature.SetStateAsync(State).ConfigureAwait(false);
        }

        public Task<PowerModeState[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

        public IAutomationStep DeepCopy() => new PowerModeAutomationStep(State);
    }
}
