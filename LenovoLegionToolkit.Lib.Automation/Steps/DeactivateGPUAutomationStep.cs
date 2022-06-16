using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DeactivateGPUAutomationStep : IAutomationStep, IDisallowDuplicatesAutomationStep
    {
        private readonly GPUController _controller = IoCContainer.Resolve<GPUController>();

        public async Task RunAsync()
        {
            if (!_controller.IsSupported())
                return;

            await _controller.RefreshAsync().ConfigureAwait(false);
            await _controller.DeactivateGPUAsync().ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new DeactivateGPUAutomationStep();
    }
}
