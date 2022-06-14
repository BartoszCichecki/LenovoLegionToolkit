using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ProcessesAreRunningAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        public string DisplayName => "When app is running";

        public ProcessInfo[] Processes { get; }

        [JsonConstructor]
        public ProcessesAreRunningAutomationPipelineTrigger(ProcessInfo[] processes) => Processes = processes;

        public Task<bool> IsSatisfiedAsync(object? context) => Task.Run(() => Processes.SelectMany(p => Process.GetProcessesByName(p.Name)).Any());

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunningAutomationPipelineTrigger(Processes);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesAreRunningAutomationPipelineTrigger running &&
                   Processes.SequenceEqual(running.Processes);
        }

        public override int GetHashCode() => HashCode.Combine(Processes);
    }
}
