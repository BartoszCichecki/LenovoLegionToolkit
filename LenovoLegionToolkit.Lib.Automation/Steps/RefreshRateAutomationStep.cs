using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class RefreshRateAutomationStep : IAutomationStep
    {
        private readonly RefreshRateFeature _feature = new();

        private readonly RefreshRate _state;

        public RefreshRateAutomationStep(RefreshRate state)
        {
            _state = state;
        }

        public async Task RunAsync()
        {
            var currentState = await _feature.GetStateAsync().ConfigureAwait(false);
            var allStates = await _feature.GetAllStatesAsync().ConfigureAwait(false);

            if (currentState == _state || !allStates.Contains(_state))
                return;

            await _feature.SetStateAsync(_state).ConfigureAwait(false);
        }
    }
}
