using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class RunAutomationStep : IAutomationStep
{
    public string? ScriptPath { get; }

    public string? ScriptArguments { get; }

    public bool RunSilently { get; }

    public bool WaitUntilFinished { get; }

    [JsonConstructor]
    public RunAutomationStep(string? scriptPath, string? scriptArguments, bool runSilently, bool waitUntilFinished)
    {
        ScriptPath = scriptPath;
        ScriptArguments = scriptArguments;
        RunSilently = runSilently;
        WaitUntilFinished = waitUntilFinished;
    }

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public async Task RunAsync(AutomationContext context, AutomationEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(ScriptPath))
            return;

        var (_, output) = await CMD.RunAsync(ScriptPath, ScriptArguments ?? string.Empty, RunSilently, WaitUntilFinished, environment.Dictionary).ConfigureAwait(false);
        context.LastRunOutput = output.TrimEnd();
    }

    IAutomationStep IAutomationStep.DeepCopy() => new RunAutomationStep(ScriptPath, ScriptArguments, RunSilently, WaitUntilFinished);
}
