using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class OnResumeAutomationPipelineTrigger : IOnResumeAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.OnResumeAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        return Task.FromResult(automationEvent is PowerStateAutomationEvent { PowerStateEvent: PowerStateEvent.Resume, PowerAdapterStateChanged: false });
    }

    public Task<bool> IsMatchingState() => Task.FromResult(false);

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.Resume = true;

    public IAutomationPipelineTrigger DeepCopy() => new OnResumeAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is OnResumeAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
