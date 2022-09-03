using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DisplayBrightnessAutomationStep : IAutomationStep
    {
        private readonly DisplayBrightnessController _controller = IoCContainer.Resolve<DisplayBrightnessController>();
        public string? Brightness { get; }

        [JsonConstructor]
        public DisplayBrightnessAutomationStep(string? brightness)
        {
            Brightness = brightness;
        }

        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public async Task RunAsync()
        {
            if (string.IsNullOrWhiteSpace(Brightness))
                return;

            if (int.TryParse(Brightness, out int brightness))
                await _controller.Set(brightness).ConfigureAwait(false);
            else
                return;
        }

        IAutomationStep IAutomationStep.DeepCopy() => new DisplayBrightnessAutomationStep(Brightness);
    }
}
