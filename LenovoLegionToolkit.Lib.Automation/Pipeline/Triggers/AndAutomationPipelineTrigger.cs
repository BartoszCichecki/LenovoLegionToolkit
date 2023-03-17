using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class AndAutomationPipelineTrigger : ICompositeAutomationPipelineTrigger
{
    public string DisplayName => string.Join(Environment.NewLine, Triggers.Select(t => t.DisplayName));

    public IAutomationPipelineTrigger[] Triggers { get; }

    [JsonConstructor]
    public AndAutomationPipelineTrigger(IAutomationPipelineTrigger[] triggers)
    {
        Triggers = triggers;
    }

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        foreach (var trigger in Triggers)
        {
            if (await trigger.IsMatchingEvent(automationEvent).ConfigureAwait(false) && await IsMatchingState().ConfigureAwait(false))
                return true;
        }

        return false;
    }

    public async Task<bool> IsMatchingState()
    {
        foreach (var trigger in Triggers)
        {
            if (!await trigger.IsMatchingState().ConfigureAwait(false))
                return false;
        }

        return true;
    }

    public IAutomationPipelineTrigger DeepCopy() => new AndAutomationPipelineTrigger(Triggers);
}