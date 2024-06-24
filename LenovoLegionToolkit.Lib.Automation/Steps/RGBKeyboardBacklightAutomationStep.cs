using System;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class RGBKeyboardBacklightAutomationStep(RGBKeyboardBacklightPreset state)
    : IAutomationStep<RGBKeyboardBacklightPreset>
{
    private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();

    public RGBKeyboardBacklightPreset State { get; } = state;

    public Task<RGBKeyboardBacklightPreset[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<RGBKeyboardBacklightPreset>());

    public Task<bool> IsSupportedAsync() => _controller.IsSupportedAsync();

    public async Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        if (!await _controller.IsSupportedAsync().ConfigureAwait(false))
            return;

        await _controller.SetLightControlOwnerAsync(true).ConfigureAwait(false);
        await _controller.SetPresetAsync(State).ConfigureAwait(false);

        MessagingCenter.Publish(new RGBKeyboardBacklightChangedMessage());
    }

    IAutomationStep IAutomationStep.DeepCopy() => new RGBKeyboardBacklightAutomationStep(State);
}
