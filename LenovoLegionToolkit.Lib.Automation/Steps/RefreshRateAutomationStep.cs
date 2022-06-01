using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class RefreshRateAutomationStep : IAutomationStep<RefreshRate>
    {
        private readonly RefreshRateFeature _feature = DIContainer.Resolve<RefreshRateFeature>();

        public RefreshRate State { get; }

        [JsonConstructor]
        public RefreshRateAutomationStep(RefreshRate state)
        {
            State = state;
        }

        public async Task RunAsync()
        {
            var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
            var allStates = await _feature.GetAllStatesAsync().ConfigureAwait(false);

            if (currentState == State || !allStates.Contains(State))
                return;

            await _feature.SetStateAsync(State).ConfigureAwait(false);
        }

        public Task<RefreshRate[]> GetAllStatesAsync() => _feature.GetAllStatesAsync();

        IAutomationStep IAutomationStep.DeepCopy() => new RefreshRateAutomationStep(State);
    }
}
