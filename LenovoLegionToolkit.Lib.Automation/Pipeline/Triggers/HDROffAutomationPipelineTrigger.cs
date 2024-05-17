using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Listeners;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class HDROffAutomationPipelineTrigger : IHDRPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.HDROffAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is HDRAutomationEvent { HDRState: HDRState.Off };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var listener = IoCContainer.Resolve<HDRListener>();
        var result = !listener.IsHDROn;
        return Task.FromResult(result && listener.IsOK);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.HDROn = false;

    public IAutomationPipelineTrigger DeepCopy() => new HDROffAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is HDROffAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
