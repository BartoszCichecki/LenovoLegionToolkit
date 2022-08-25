using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ProcessesStopRunningAutomationPipelineTrigger : IAutomationPipelineTrigger, IProcessesAutomationPipelineTrigger
    {
        public string DisplayName => "When app closes";

        public ProcessInfo[] Processes { get; }

        [JsonConstructor]
        public ProcessesStopRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is StartupAutomationEvent)
                return Task.FromResult(false);

            if (automationEvent is not ProcessAutomationEvent pae || pae.ProcessEventInfo.Type != ProcessEventInfoType.Stopped)
                return Task.FromResult(false);

            var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).IsEmpty();
            return Task.FromResult(result);
        }

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesStopRunningAutomationPipelineTrigger(Processes);

        public IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesStopRunningAutomationPipelineTrigger(processes);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesStopRunningAutomationPipelineTrigger running &&
                   Processes.SequenceEqual(running.Processes);
        }

        public override int GetHashCode() => HashCode.Combine(Processes);
    }
}
