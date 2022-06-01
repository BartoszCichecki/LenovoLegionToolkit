using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ScriptAutomationStep : IAutomationStep
    {
        public string? ScriptPath { get; }

        public string? ScriptArguments { get; }

        [JsonConstructor]
        public ScriptAutomationStep(string? scriptPath, string? scriptArguments)
        {
            ScriptPath = scriptPath;
            ScriptArguments = scriptArguments;
        }

        public async Task RunAsync()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath))
                return;

            await CMD.RunAsync(ScriptPath, ScriptArguments ?? "").ConfigureAwait(false);
        }

        IAutomationStep IAutomationStep.DeepCopy() => new ScriptAutomationStep(ScriptPath, ScriptArguments);
    }
}
