using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class RunAutomationStep : IAutomationStep
{
    public string? ScriptPath { get; }

    public string? ScriptArguments { get; }

    [JsonConstructor]
    public RunAutomationStep(string? scriptPath, string? scriptArguments)
    {
        ScriptPath = scriptPath;
        ScriptArguments = scriptArguments;
    }

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task RunAsync()
    {
        if (string.IsNullOrWhiteSpace(ScriptPath))
            return;

        await CMD.RunAsync(ScriptPath, ScriptArguments ?? "", false).ConfigureAwait(false);
    }

    IAutomationStep IAutomationStep.DeepCopy() => new RunAutomationStep(ScriptPath, ScriptArguments);
}
