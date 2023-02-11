using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class ExternalDisplayConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, INativeWindowsMessagePipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
{
    [JsonIgnore] public string DisplayName => Resource.ExternalDisplayConnectedAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.MonitorConnected };
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new ExternalDisplayConnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is ExternalDisplayConnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}

public class DisplayOnAutomationPipelineTrigger : IAutomationPipelineTrigger, INativeWindowsMessagePipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
{
    [JsonIgnore] public string DisplayName => "When displays turn on";

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.MonitorOn };
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new DisplayOnAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is DisplayOnAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}

public class DisplayOffAutomationPipelineTrigger : IAutomationPipelineTrigger, INativeWindowsMessagePipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
{
    [JsonIgnore] public string DisplayName => "When displays turn off";

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.MonitorOff };
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new DisplayOffAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is DisplayOnAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
