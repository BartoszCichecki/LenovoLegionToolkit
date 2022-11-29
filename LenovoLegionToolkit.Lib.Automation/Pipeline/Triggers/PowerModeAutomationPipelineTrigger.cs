using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class PowerModeAutomationPipelineTrigger : IAutomationPipelineTrigger, IPowerModeAutomationPipelineTrigger
{
    public string DisplayName => Resource.PowerModeAutomationPipelineTrigger_DisplayName;

    public PowerModeState PowerModeState { get; }

    [JsonConstructor]
    public PowerModeAutomationPipelineTrigger(PowerModeState powerModeState)
    {
        PowerModeState = powerModeState;
    }

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        if (automationEvent is StartupAutomationEvent)
            return Task.FromResult(false);

        if (automationEvent is not PowerModeAutomationEvent pmae)
            return Task.FromResult(false);

        var result = pmae.PowerModeState == PowerModeState;
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new PowerModeAutomationPipelineTrigger(PowerModeState);

    public IAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState) => new PowerModeAutomationPipelineTrigger(powerModeState);

    public override bool Equals(object? obj)
    {
        return obj is PowerModeAutomationPipelineTrigger t && PowerModeState == t.PowerModeState;
    }

    public override int GetHashCode() => HashCode.Combine(PowerModeState);
}