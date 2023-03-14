using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class LidClosedAutomationPipelineTrigger : INativeWindowsMessagePipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.LidClosedAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.LidClosed };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        throw new NotImplementedException();
    }

    public IAutomationPipelineTrigger DeepCopy() => new LidClosedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is LidClosedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
