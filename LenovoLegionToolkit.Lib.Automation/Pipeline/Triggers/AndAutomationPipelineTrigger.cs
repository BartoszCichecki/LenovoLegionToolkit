using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

[method: JsonConstructor]
public class AndAutomationPipelineTrigger(IAutomationPipelineTrigger[] triggers) : ICompositeAutomationPipelineTrigger
{
    public string DisplayName => string.Join(Environment.NewLine, Triggers.Select(t => t.DisplayName));

    public IAutomationPipelineTrigger[] Triggers { get; } = triggers;

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        foreach (var trigger in Triggers)
        {
            if (!await trigger.IsMatchingEvent(automationEvent).ConfigureAwait(false))
                continue;

            foreach (var otherTrigger in Triggers.Where(t => !ReferenceEquals(t, trigger)))
            {
                if (!await otherTrigger.IsMatchingState().ConfigureAwait(false))
                    return false;
            }

            return true;
        }

        return false;
    }

    public Task<bool> IsMatchingState() => Task.FromResult(false);

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        foreach (var trigger in Triggers)
            trigger.UpdateEnvironment(environment);
    }

    public IAutomationPipelineTrigger DeepCopy() => new AndAutomationPipelineTrigger(Triggers);
}
