using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class ExternalDisplayConnectedAutomationPipelineTrigger : INativeWindowsMessagePipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.ExternalDisplayConnectedAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.ExternalMonitorConnected };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = ExternalDisplays.Get().Length > 0;
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.ExternalDisplayConnected = true;

    public IAutomationPipelineTrigger DeepCopy() => new ExternalDisplayConnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is ExternalDisplayConnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
