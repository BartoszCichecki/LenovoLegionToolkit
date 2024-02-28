using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class PeriodicAutomationPipelineTrigger : IPeriodicAutomationPipelineTrigger
{
    public string DisplayName => Resource.PeriodicActionPipelineTrigger_DisplayName;

    public TimeSpan Period { get; }

    [JsonConstructor]
    public PeriodicAutomationPipelineTrigger(TimeSpan period)
    {
        Period = period;
    }

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        return automationEvent is not TimeAutomationEvent ? Task.FromResult(false) : IsMatching();
    }

    public Task<bool> IsMatchingState() => IsMatching();

    public IAutomationPipelineTrigger DeepCopy() => new PeriodicAutomationPipelineTrigger(Period);

    public IPeriodicAutomationPipelineTrigger DeepCopy(TimeSpan period) => new PeriodicAutomationPipelineTrigger(period);

    private Task<bool> IsMatching()
    {
        var currentDayMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;
        var isPeriod = currentDayMinutes % Period.TotalMinutes == 0;

        return Task.FromResult(isPeriod);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) { }
}
