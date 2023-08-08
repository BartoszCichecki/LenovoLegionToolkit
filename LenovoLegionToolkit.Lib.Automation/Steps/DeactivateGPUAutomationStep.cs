﻿using System;
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

    public async Task RunAsync(AutomationEnvironment _)
    {
        if (!_controller.IsSupported())
            return;

        var status = await _controller.RefreshNowAsync().ConfigureAwait(false);

        switch (State)
        {
            case DeactivateGPUAutomationStepState.KillApps when status.State is GPUController.GPUState.Active:
                await _controller.KillGPUProcessesAsync().ConfigureAwait(false);
                break;
            case DeactivateGPUAutomationStepState.RestartGPU when status.State is GPUController.GPUState.Active or GPUController.GPUState.Inactive:
                await _controller.RestartGPUAsync().ConfigureAwait(false);
                break;
        }
    }

    IAutomationStep IAutomationStep.DeepCopy() => new DeactivateGPUAutomationStep(State);
}
