using System;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class QuickActionAutomationStep(Guid? pipelineId)
    : IAutomationStep
{
    public Guid? PipelineId { get; } = pipelineId;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token) => Task.CompletedTask;

    IAutomationStep IAutomationStep.DeepCopy() => new QuickActionAutomationStep(PipelineId);
}
