using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Steps;

namespace LenovoLegionToolkit.Lib.Automation
{

    internal class AutomationPipeline
    {
        private readonly List<AutomationPipelineCriteria> _criteria;
        private readonly List<IAutomationStep> _steps;

        public AutomationPipeline(List<AutomationPipelineCriteria> criteria, List<IAutomationStep> steps)
        {
            _criteria = criteria;
            _steps = steps;
        }

        public async Task RunAsync()
        {
            if (!_criteria.All(t => t.IsSatisfied()))
                return;

            foreach (var step in _steps)
                await step.RunAsync().ConfigureAwait(false);
        }
    }
}
