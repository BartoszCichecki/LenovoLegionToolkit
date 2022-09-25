using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DisplayBrightnessAutomationStep : IAutomationStep
    {
        private readonly DisplayBrightnessController _controller = IoCContainer.Resolve<DisplayBrightnessController>();
        public int Brightness { get; }

        [JsonConstructor]
        public DisplayBrightnessAutomationStep(int brightness)
        {
            Brightness = brightness;
        }

        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public Task RunAsync()
        {
            return _controller.SetBrightnessAsync(Brightness);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new DisplayBrightnessAutomationStep(Brightness);
    }
}
