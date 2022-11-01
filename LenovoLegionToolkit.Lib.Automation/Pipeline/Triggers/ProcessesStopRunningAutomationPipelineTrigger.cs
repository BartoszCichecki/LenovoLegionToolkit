using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ProcessesStopRunningAutomationPipelineTrigger : IAutomationPipelineTrigger, IProcessesAutomationPipelineTrigger
    {
        public string DisplayName => Resource.ProcessesStopRunningAutomationPipelineTrigger_DisplayName;

        public ProcessInfo[] Processes { get; }

        [JsonConstructor]
        public ProcessesStopRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is StartupAutomationEvent)
                return Task.FromResult(false);

            if (automationEvent is not ProcessAutomationEvent { ProcessEventInfo.Type: ProcessEventInfoType.Stopped } pae)
                return Task.FromResult(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Checking for {pae.ProcessEventInfo.Process.Name}... [processes={string.Join(",", Processes.Select(p => p.Name))}]");

            if (!Processes.Contains(pae.ProcessEventInfo.Process) && !Processes.Select(p => p.Name).Contains(pae.ProcessEventInfo.Process.Name))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Process name {pae.ProcessEventInfo.Process.Name} not in the list.");

                return Task.FromResult(false);
            }

            var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).IsEmpty();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process name {pae.ProcessEventInfo.Process.Name} found in process list: {!result}.");

            return Task.FromResult(result);
        }

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesStopRunningAutomationPipelineTrigger(Processes);

        public IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesStopRunningAutomationPipelineTrigger(processes);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesStopRunningAutomationPipelineTrigger t && Processes.SequenceEqual(t.Processes);
        }

        public override int GetHashCode() => HashCode.Combine(Processes);
    }
}
