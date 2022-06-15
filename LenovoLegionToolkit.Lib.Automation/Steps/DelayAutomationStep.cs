using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class DelayAutomationStep : IAutomationStep<Delay>
    {
        public Delay State { get; }

        [JsonConstructor]
        public DelayAutomationStep(Delay state) => State = state;

        public Task<Delay[]> GetAllStatesAsync() => Task.FromResult(new Delay[] {
            new(1),
            new(2),
            new(3),
        });

        public IAutomationStep DeepCopy() => new DelayAutomationStep(State);

        public Task RunAsync() => Task.Delay(State.DelaySeconds * 1000);
    }
}
