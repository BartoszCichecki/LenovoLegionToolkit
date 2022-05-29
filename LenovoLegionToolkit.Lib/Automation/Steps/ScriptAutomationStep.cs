using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ScriptAutomationStep : IAutomationStep
    {
        private readonly string _scriptPath;
        private readonly string _scriptParameters;

        public ScriptAutomationStep(string scriptPath, string scriptParameters)
        {
            _scriptPath = scriptPath;
            _scriptParameters = scriptParameters;
        }

        public async Task RunAsync()
        {
            await CMD.RunAsync(_scriptPath, _scriptParameters).ConfigureAwait(false);
        }
    }
}
