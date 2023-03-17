using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Features;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class PowerModeAutomationPipelineTrigger : IPowerModeAutomationPipelineTrigger
{
    public string DisplayName => Resource.PowerModeAutomationPipelineTrigger_DisplayName;

    public PowerModeState PowerModeState { get; }

    [JsonConstructor]
    public PowerModeAutomationPipelineTrigger(PowerModeState powerModeState)
    {
        PowerModeState = powerModeState;
    }

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not PowerModeAutomationEvent e)
            return Task.FromResult(false);

        var result = e.PowerModeState == PowerModeState;
        return Task.FromResult(result);
    }

    public async Task<bool> IsMatchingState()
    {
        var feature = IoCContainer.Resolve<PowerModeFeature>();
        return await feature.GetStateAsync().ConfigureAwait(false) == PowerModeState;
    }

    public IAutomationPipelineTrigger DeepCopy() => new PowerModeAutomationPipelineTrigger(PowerModeState);

    public IPowerModeAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState) => new PowerModeAutomationPipelineTrigger(powerModeState);

    public override bool Equals(object? obj)
    {
        return obj is PowerModeAutomationPipelineTrigger t && PowerModeState == t.PowerModeState;
    }

    public override int GetHashCode() => HashCode.Combine(PowerModeState);

    public override string ToString() => $"{nameof(PowerModeState)}: {PowerModeState}";
}