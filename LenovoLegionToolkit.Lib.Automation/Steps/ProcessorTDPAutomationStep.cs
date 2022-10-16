using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Controllers;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ProcessorTDPAutomationStep : IAutomationStep
    {
        private readonly ProcessorController _controller = IoCContainer.Resolve<ProcessorController>();
        
        private ProcessorManager? _manager;

        public ProcessorTDPState State { get; }

        [JsonConstructor]
        public ProcessorTDPAutomationStep(ProcessorTDPState state) => State = state;
        
        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public async Task RunAsync()
        {
            ProcessorController processor = _controller.GetCurrent();
            _manager = IoCContainer.Resolve<ProcessorManager>();

            if (State.Stapm != 0)
                processor.SetTDPLimit(PowerType.Stapm, State.Stapm);
            if (State.Fast != 0)
                processor.SetTDPLimit(PowerType.Fast, State.Fast);
            if (State.Slow != 0)
                processor.SetTDPLimit(PowerType.Slow, State.Slow);
            if (State.UseMSR)
            {
                if (processor.GetType() == typeof(IntelProcessorController))
                {
                    ((IntelProcessorController)processor).SetMSRLimits(State.Slow, State.Fast);
                }
            }
            if (State.MaintainTDP)
            {
                await _manager.StartAsync(State.Stapm, State.Fast, State.Slow, State.UseMSR, State.Interval).ConfigureAwait(false);
            }
            else
            {
                await _manager.StopAsync().ConfigureAwait(false);
            }
        }

        IAutomationStep IAutomationStep.DeepCopy() => new ProcessorTDPAutomationStep(State);
    }
}
