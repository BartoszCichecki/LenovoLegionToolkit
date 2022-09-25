using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class RGBKeyboardBacklightAutomationStep : IAutomationStep<RGBKeyboardBacklightPreset>
    {
        private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();

        public RGBKeyboardBacklightPreset State { get; }

        public RGBKeyboardBacklightAutomationStep(RGBKeyboardBacklightPreset state) => State = state;

        public Task<RGBKeyboardBacklightPreset[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<RGBKeyboardBacklightPreset>());

        public Task<bool> IsSupportedAsync() => Task.FromResult(_controller.IsSupported());

        public async Task RunAsync()
        {
            if (!_controller.IsSupported())
                return;

            await _controller.SetLightControlOwnerAsync(true).ConfigureAwait(false);
            await _controller.SetPresetAsync(State).ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new RGBKeyboardBacklightAutomationStep(State);
    }
}
