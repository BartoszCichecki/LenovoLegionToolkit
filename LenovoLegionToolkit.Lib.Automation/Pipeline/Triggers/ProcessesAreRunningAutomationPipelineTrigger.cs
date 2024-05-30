﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

[method: JsonConstructor]
public class ProcessesAreRunningAutomationPipelineTrigger(ProcessInfo[]? processes) : IProcessesAutomationPipelineTrigger
{
    public string DisplayName => Resource.ProcessesAreRunningAutomationPipelineTrigger_DisplayName;

    public ProcessInfo[] Processes { get; } = processes ?? [];

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not ProcessAutomationEvent { Type: ProcessEventInfoType.Started } e)
            return Task.FromResult(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Checking for {e.ProcessInfo.Name}... [processes={string.Join(",", Processes.Select(p => p.Name))}]");

        if (!Processes.Contains(e.ProcessInfo) && !Processes.Select(p => p.Name).Contains(e.ProcessInfo.Name))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process name {e.ProcessInfo.Name} not in the list.");

            return Task.FromResult(false);
        }

        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Process name {e.ProcessInfo.Name} found in process list: {result}.");

        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any();
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.ProcessesStarted = true;
        environment.Processes = Processes;
    }

    public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunningAutomationPipelineTrigger(Processes);

    public IProcessesAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesAreRunningAutomationPipelineTrigger(processes);

    public override bool Equals(object? obj)
    {
        return obj is ProcessesAreRunningAutomationPipelineTrigger t && Processes.SequenceEqual(t.Processes);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();
        Processes.ForEach(p => hc.Add(p));
        return hc.ToHashCode();
    }

    public override string ToString() => $"{nameof(Processes)}: {string.Join(", ", Processes)}";
}
