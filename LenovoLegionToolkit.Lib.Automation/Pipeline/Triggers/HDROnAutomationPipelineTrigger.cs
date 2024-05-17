using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Listeners;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class HDROnAutomationPipelineTrigger : IHDRPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.HDROnAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is HDRAutomationEvent { IsHDROn: true };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var listener = IoCContainer.Resolve<DisplayConfigurationListener>();
        var result = listener.IsHDROn;
        return Task.FromResult(result.HasValue && result.Value);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.HDROn = true;

    public IAutomationPipelineTrigger DeepCopy() => new HDROnAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is HDROnAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
