using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline
{
    public class AutomationPipeline
    {
        public string? Name { get; set; }

        public AutomationPipelineTrigger? Trigger { get; set; }

        public List<IAutomationStep> Steps { get; set; } = new();

        public bool IsExclusive { get; set; }

        public AutomationPipeline() { }

        public AutomationPipeline(string name) => Name = name;

        public AutomationPipeline(AutomationPipelineTrigger trigger) => Trigger = trigger;

        public bool IsTriggerSatisfied() => Trigger?.IsSatisfied() ?? false;

        internal async Task RunAsync(CancellationToken token = default)
        {
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
            Trigger = Trigger,
            Steps = Steps.Select(s => s.DeepCopy()).ToList(),
            IsExclusive = IsExclusive,
        };
    }
}
