using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class RGBKeyboardBacklightAutomationStep : IAutomationStep<RGBKeyboardBacklightSelectedPreset>, IDisallowDuplicatesAutomationStep
    {
        private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();

        public RGBKeyboardBacklightSelectedPreset State { get; }

        public RGBKeyboardBacklightAutomationStep(RGBKeyboardBacklightSelectedPreset state) => State = state;

        public Task<RGBKeyboardBacklightSelectedPreset[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<RGBKeyboardBacklightSelectedPreset>());

        public Task<bool> IsSupportedAsync() => Task.FromResult(_controller.IsSupported());

        public async Task RunAsync()
        {
            if (!_controller.IsSupported())
                return;

            await _controller.SetPresetAsync(State).ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new RGBKeyboardBacklightAutomationStep(State);
    }
}
