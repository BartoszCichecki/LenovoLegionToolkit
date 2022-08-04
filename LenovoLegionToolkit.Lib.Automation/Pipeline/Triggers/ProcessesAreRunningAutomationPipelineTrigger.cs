using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ProcessesAreRunningAutomationPipelineTrigger : IAutomationPipelineTrigger, IProcessesAutomationPipelineTrigger
    {
        public string DisplayName => "When app is running";

        public ProcessInfo[] Processes { get; }

        [JsonConstructor]
        public ProcessesAreRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

        public bool IsSatisfied(object? context)
        {
            if (context is not ProcessEventInfo pei || pei.Type != ProcessEventInfoType.Started)
                return false;

            var result = Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any();
            return result;
        }

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunningAutomationPipelineTrigger(Processes);

        public IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes) => new ProcessesAreRunningAutomationPipelineTrigger(processes);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesAreRunningAutomationPipelineTrigger running &&
                   Processes.SequenceEqual(running.Processes);
        }

        public override int GetHashCode() => HashCode.Combine(Processes);
    }
}
