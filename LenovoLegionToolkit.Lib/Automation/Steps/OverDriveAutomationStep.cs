using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class OverDriveAutomationStep : IAutomationStep
    {
        private readonly OverDriveFeature _feature = new();
        private readonly OverDriveState _state;

        public OverDriveAutomationStep(OverDriveState state)
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
    }
}
