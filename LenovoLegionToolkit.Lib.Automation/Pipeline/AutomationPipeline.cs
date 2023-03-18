using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline;

public class AutomationPipeline
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? IconName { get; set; }

    public string? Name { get; set; }

    public IAutomationPipelineTrigger? Trigger { get; set; }

    public List<IAutomationStep> Steps { get; set; } = new();

    public bool IsExclusive { get; set; } = true;

    public IEnumerable<IAutomationPipelineTrigger> AllTriggers
    {
        get
        {
            if (Trigger is not null && Trigger is not ICompositeAutomationPipelineTrigger)
                yield return Trigger;

            if (Trigger is ICompositeAutomationPipelineTrigger compositeTrigger)
                foreach (var trigger in compositeTrigger.Triggers)
                    yield return trigger;
        }
    }

    public AutomationPipeline() { }

    public AutomationPipeline(string name) => Name = name;

    public AutomationPipeline(IAutomationPipelineTrigger trigger) => Trigger = trigger;

    internal async Task RunAsync(CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipeline interrupted.");
            return;
        }

        var stepExceptions = new List<Exception>();

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

            try
            {
                await step.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Step run failed. [name={step.GetType().Name}]", ex);

                stepExceptions.Add(ex);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Step completed successfully. [type={step.GetType().Name}]");
        }

        if (stepExceptions.Any())
            throw new AggregateException(stepExceptions);
    }

    public AutomationPipeline DeepCopy() => new()
    {
        Id = Id,
        IconName = IconName,
        Name = Name,
        Trigger = Trigger?.DeepCopy(),
        Steps = Steps.Select(s => s.DeepCopy()).ToList(),
        IsExclusive = IsExclusive,
    };
}