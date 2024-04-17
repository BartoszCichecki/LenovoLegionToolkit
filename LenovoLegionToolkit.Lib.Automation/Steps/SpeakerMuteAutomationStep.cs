using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class SpeakerMuteAutomationStep : IAutomationStep
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        Speaker.Mute();
        return Task.CompletedTask;
    }

    public IAutomationStep DeepCopy() => new SpeakerMuteAutomationStep();
}
