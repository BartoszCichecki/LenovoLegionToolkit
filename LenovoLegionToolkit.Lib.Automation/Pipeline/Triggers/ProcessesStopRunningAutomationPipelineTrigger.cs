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

        public async Task<bool> IsSatisfiedAsync(object? context)
        {
            if (context is not ProcessEventInfo pei || pei.Type != ProcessEventInfoType.Stopped)
                return false;

            var matches = Processes.Contains(pei.Process);
            if (!matches)
                return false;

            var result = await Task.Run(() => Processes.SelectMany(pn => Process.GetProcessesByName(pn.Name)).IsEmpty()).ConfigureAwait(false);
            return result;
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
