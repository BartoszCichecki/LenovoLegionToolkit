using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ProcessAutomationStep : IAutomationStep
    {
        public ProcessAutomationState State;

        [JsonConstructor]
        public ProcessAutomationStep(ProcessAutomationState state) => State = state;
        
        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public async Task RunAsync()
        {

            if (!State.Processes.Any())
                return;

            if (State.State == ProcessState.Start)
                foreach (ProcessInfo process in State.Processes)
                    await CMD.RunAsync(process.ExecutablePath,"").ConfigureAwait(false);

            if (State.State == ProcessState.Stop)
                foreach (ProcessInfo process in State.Processes)
                {
                    Process[] ps = Process.GetProcessesByName(process.Name);
                    foreach (Process p in ps)
                        p.Kill();
                }
        }

        IAutomationStep IAutomationStep.DeepCopy() => new ProcessAutomationStep(State);
    }
}
