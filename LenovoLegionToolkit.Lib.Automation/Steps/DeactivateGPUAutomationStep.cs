using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class DeactivateGPUAutomationStep : IAutomationStep<DeactivateGPUAutomationStepState>
{
    private readonly GPUController _controller = IoCContainer.Resolve<GPUController>();

    public DeactivateGPUAutomationStepState State { get; }

    public DeactivateGPUAutomationStep(DeactivateGPUAutomationStepState state) => State = state;

    public Task<DeactivateGPUAutomationStepState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<DeactivateGPUAutomationStepState>());

    public Task<bool> IsSupportedAsync() => Task.FromResult(_controller.IsSupported());

    public async Task RunAsync()
    {
        if (!_controller.IsSupported())
            return;

        var status = await _controller.RefreshNowAsync().ConfigureAwait(false);

        if (!status.CanBeDeactivated)
            return;

        switch (State)
        {
            case DeactivateGPUAutomationStepState.KillApps:
                await _controller.KillGPUProcessesAsync().ConfigureAwait(false);
                break;
            case DeactivateGPUAutomationStepState.RestartGPU:
                await _controller.DeactivateGPUAsync().ConfigureAwait(false);
                break;
        }
    }

    IAutomationStep IAutomationStep.DeepCopy() => new DeactivateGPUAutomationStep(State);
}