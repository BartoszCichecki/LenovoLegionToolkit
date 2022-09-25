using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ProcessesAreRunningAutomationPipelineTrigger : IAutomationPipelineTrigger, IProcessesAutomationPipelineTrigger
    {
        public string DisplayName => "When app is running";

        public ProcessInfo[] Processes { get; }

        [JsonConstructor]
        public ProcessesAreRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is StartupAutomationEvent)
                return Task.FromResult(false);

            if (automationEvent is not ProcessAutomationEvent { ProcessEventInfo.Type: ProcessEventInfoType.Started })
                return Task.FromResult(false);

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
    }
}
