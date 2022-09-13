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

        public async Task RunAsync()
        {
            if (Brightness == 0)
                return;

            await _controller.SetBrightnessAsync(Brightness).ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new DisplayBrightnessAutomationStep(Brightness);
    }
}
