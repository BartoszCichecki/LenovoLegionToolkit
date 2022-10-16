using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ProcessAutomationStep : IAutomationStep
    {
        public ProcessAutomationState State { get; }

        [JsonConstructor]
        public ProcessAutomationStep(ProcessAutomationState state) => State = state;

        public Task<ProcessState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<ProcessState>());

        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public async Task RunAsync()
        {

           if (State.Processes == null)
                return;

            switch (State.State)
            {
                case ProcessState.Start:
                    foreach (ProcessInfo process in State.Processes)
                        await CMD.RunAsync(process.ExecutablePath, "").ConfigureAwait(false);
                    break;
                case ProcessState.Stop:
                    foreach (ProcessInfo process in State.Processes)
                    {
                        Process[] ps = Process.GetProcessesByName(process.Name);
                        foreach (Process p in ps)
                            p.Kill();
                    }
                    break;
            }
        }

        IAutomationStep IAutomationStep.DeepCopy() => new ProcessAutomationStep(State);
    }
}
