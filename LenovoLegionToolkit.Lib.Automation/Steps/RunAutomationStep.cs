using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class RunAutomationStep(string? scriptPath, string? scriptArguments)
    : IAutomationStep
{
    public string? ScriptPath { get; } = scriptPath;

    public string? ScriptArguments { get; } = scriptArguments;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task RunAsync(AutomationContext context, AutomationEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(ScriptPath))
            return;

        var (_, output) = await CMD.RunAsync(ScriptPath, ScriptArguments ?? string.Empty, environment: environment.Dictionary).ConfigureAwait(false);
        context.LastRunOutput = output.TrimEnd();
    }

    IAutomationStep IAutomationStep.DeepCopy() => new RunAutomationStep(ScriptPath, ScriptArguments);
}
