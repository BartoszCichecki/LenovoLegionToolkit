using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class PowerModeAutomationStep : IAutomationStep<PowerModeState>
    {
        private readonly PowerModeFeature _feature = IoCContainer.Resolve<PowerModeFeature>();

        public PowerModeState State { get; }

        [JsonConstructor]
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

        IAutomationStep IAutomationStep.DeepCopy() => new PowerModeAutomationStep(State);
    }
}
