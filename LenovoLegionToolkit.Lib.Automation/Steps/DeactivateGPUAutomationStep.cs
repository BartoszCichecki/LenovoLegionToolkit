using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DeactivateGPUAutomationStep : IAutomationStep
    {
        private readonly GPUController _controller = DIContainer.Resolve<GPUController>();

        public async Task RunAsync()
        {
            await Task.Delay(5000).ConfigureAwait(false);
            await _controller.DeactivateGPUAsync().ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new DeactivateGPUAutomationStep();
    }
}
