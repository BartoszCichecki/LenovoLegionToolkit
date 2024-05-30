using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

[method: JsonConstructor]
public class ProcessesStopRunningAutomationPipelineTrigger(ProcessInfo[]? processes)
    : IProcessesAutomationPipelineTrigger
{
    public string DisplayName => Resource.ProcessesStopRunningAutomationPipelineTrigger_DisplayName;

    public ProcessInfo[] Processes { get; } = processes ?? [];

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not ProcessAutomationEvent { Type: ProcessEventInfoType.Stopped } e)
            return Task.FromResult(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Checking for {e.ProcessInfo.Name}... [processes={string.Join(", ", Processes.Select(p => p.Name))}]");

        if (!Processes.Contains(e.ProcessInfo) && !Processes.Select(p => p.Name).Contains(e.ProcessInfo.Name))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process name {e.ProcessInfo.Name} not in the list.");

            return Task.FromResult(false);
        }

        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).IsEmpty();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Process name {e.ProcessInfo.Name} found in process list: {!result}.");

        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).IsEmpty();
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.ProcessesStarted = false;
        environment.Processes = Processes;
    }

    public IAutomationPipelineTrigger DeepCopy() => new ProcessesStopRunningAutomationPipelineTrigger(Processes);

    public IProcessesAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesStopRunningAutomationPipelineTrigger(processes);

    public override bool Equals(object? obj)
    {
        return obj is ProcessesStopRunningAutomationPipelineTrigger t && Processes.SequenceEqual(t.Processes);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();
        Processes.ForEach(p => hc.Add(p));
        return hc.ToHashCode();
    }

    public override string ToString() => $"{nameof(Processes)}: {string.Join(", ", Processes)}";
}
