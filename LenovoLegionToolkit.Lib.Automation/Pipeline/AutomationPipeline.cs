﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline;

public class AutomationPipeline
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? IconName { get; set; }

    public string? Name { get; set; }

    public IAutomationPipelineTrigger? Trigger { get; set; }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public List<IAutomationStep> Steps { get; set; } = new();

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public bool IsExclusive { get; set; } = true;

    [JsonIgnore]
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

            var environment = new AutomationEnvironment();
            AllTriggers.ForEach(t => t.UpdateEnvironment(ref environment));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Running step... [type={step.GetType().Name}]");

            try
            {
                await step.RunAsync(environment).ConfigureAwait(false);
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
