using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DeactivateGPUAutomationStep : IAutomationStep, IDisallowDuplicatesAutomationStep
    {
        private readonly GPUController _controller = IoCContainer.Resolve<GPUController>();

        public Task RunAsync() => _controller.DeactivateGPUAsync();

        IAutomationStep IAutomationStep.DeepCopy() => new DeactivateGPUAutomationStep();
    }
}
