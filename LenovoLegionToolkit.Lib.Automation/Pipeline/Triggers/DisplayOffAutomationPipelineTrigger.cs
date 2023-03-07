using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class DisplayOffAutomationPipelineTrigger : INativeWindowsMessagePipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.DisplayOffAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.MonitorOff };
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new DisplayOffAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is DisplayOnAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
