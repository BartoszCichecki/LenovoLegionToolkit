using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class ExternalDisplayDisconnectedAutomationPipelineTrigger : INativeWindowsMessagePipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
{
    [JsonIgnore] public string DisplayName => Resource.ExternalDisplayDisconnectedAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.ExternalMonitorDisconnected };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = ExternalDisplays.Get().Length < 1;
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.ExternalDisplayConnected = false;

    public IAutomationPipelineTrigger DeepCopy() => new ExternalDisplayDisconnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is ExternalDisplayDisconnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
