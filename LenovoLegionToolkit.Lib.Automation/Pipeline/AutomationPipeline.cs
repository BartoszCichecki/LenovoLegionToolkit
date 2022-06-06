using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline
{
    public class AutomationPipeline
    {
        public string? Name { get; set; }

        public List<AutomationPipelineTrigger> Triggers { get; set; } = new();

        public List<IAutomationStep> Steps { get; set; } = new();

        public AutomationPipeline() { }

        public AutomationPipeline(string name) => Name = name;

        public AutomationPipeline(AutomationPipelineTrigger trigger) => Triggers.Add(trigger);

        internal async Task RunAsync(bool force = false, CancellationToken token = default)
        {
            if (!force && Triggers.IsEmpty() && !Triggers.All(t => t.IsSatisfied()))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Triggers not satisfied.");
                return;
            }

            if (token.IsCancellationRequested)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipeline interrupted.");
                return;
            }

            foreach (var step in Steps)
            {
                if (token.IsCancellationRequested)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Pipeline interrupted.");
                    break;
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Running step... [type={step.GetType().Name}]");

                await step.RunAsync().ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Step completed successfully. [type={step.GetType().Name}]");
            }
        }

        internal AutomationPipeline DeepCopy() => new()
        {
            Name = Name,
            Triggers = Triggers.ToList(),
            Steps = Steps.Select(s => s.DeepCopy()).ToList(),
        };
    }
}
