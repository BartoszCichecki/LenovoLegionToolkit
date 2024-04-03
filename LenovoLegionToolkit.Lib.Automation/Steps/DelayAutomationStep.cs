using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class DelayAutomationStep(Delay state)
    : IAutomationStep<Delay>
{
    public Delay State { get; } = state;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task<Delay[]> GetAllStatesAsync() => Task.FromResult(new Delay[] {
        new(1),
        new(2),
        new(3),
        new(5)
    });

    public IAutomationStep DeepCopy() => new DelayAutomationStep(State);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token) => Task.Delay(TimeSpan.FromSeconds(State.DelaySeconds), token);
}
