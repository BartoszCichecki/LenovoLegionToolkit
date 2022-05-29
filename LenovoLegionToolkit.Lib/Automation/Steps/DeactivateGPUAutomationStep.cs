using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DeactivateGPUAutomationStep : IAutomationStep
    {
        private readonly GPUController _controller = new();

        public async Task RunAsync()
        {
            await Task.Delay(5000).ConfigureAwait(false);
            await _controller.DeactivateGPUAsync().ConfigureAwait(false);
        }
    }
}
