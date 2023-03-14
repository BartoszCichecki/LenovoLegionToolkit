using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class ProcessesAreRunningAutomationPipelineTrigger : IProcessesAutomationPipelineTrigger
{
    public string DisplayName => Resource.ProcessesAreRunningAutomationPipelineTrigger_DisplayName;

    public ProcessInfo[] Processes { get; }

    [JsonConstructor]
    public ProcessesAreRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not ProcessAutomationEvent { ProcessEventInfo.Type: ProcessEventInfoType.Started } e)
            return Task.FromResult(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Checking for {e.ProcessEventInfo.Process.Name}... [processes={string.Join(",", Processes.Select(p => p.Name))}]");

        if (!Processes.Contains(e.ProcessEventInfo.Process) && !Processes.Select(p => p.Name).Contains(e.ProcessEventInfo.Process.Name))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process name {e.ProcessEventInfo.Process.Name} not in the list.");

            return Task.FromResult(false);
        }

        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Process name {e.ProcessEventInfo.Process.Name} found in process list: {result}.");

        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any();
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunningAutomationPipelineTrigger(Processes);

    public IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesAreRunningAutomationPipelineTrigger(processes);

    public override bool Equals(object? obj)
    {
        return obj is ProcessesAreRunningAutomationPipelineTrigger t && Processes.SequenceEqual(t.Processes);
    }

    public override int GetHashCode() => HashCode.Combine(Processes);

    public override string ToString() => $"{nameof(Processes)}: {string.Join(", ", Processes)}";
}