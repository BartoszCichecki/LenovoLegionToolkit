using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Steps;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline
{
    public class AutomationPipeline
    {
        public List<AutomationPipelineTrigger> Triggers { get; set; } = new();

        public List<IAutomationStep> Steps { get; set; } = new();

        public AutomationPipeline() { }

        public AutomationPipeline(AutomationPipelineTrigger trigger)
        {
            Triggers.Add(trigger);
        }

        internal async Task RunAsync(bool force = false)
        {
            if (!force && !Triggers.All(t => t.IsSatisfied()))
                return;

            foreach (var step in Steps)
                await step.RunAsync().ConfigureAwait(false);
        }

        internal AutomationPipeline DeepCopy() => new()
        {
            Triggers = Triggers.ToList(),
            Steps = Steps.Select(s => s.DeepCopy()).ToList(),
        };
    }
}
