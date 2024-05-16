using System;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Macro;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class MacroAutomationStep(MacroAutomationStepState state) : IAutomationStep<MacroAutomationStepState>
{
    private readonly MacroController _controller = IoCContainer.Resolve<MacroController>();

    public MacroAutomationStepState State { get; set; } = state;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task<MacroAutomationStepState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<MacroAutomationStepState>());

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        _controller.SetEnabled(State is MacroAutomationStepState.On);
        return Task.CompletedTask;
    }

    public IAutomationStep DeepCopy() => new MacroAutomationStep(State);

}
