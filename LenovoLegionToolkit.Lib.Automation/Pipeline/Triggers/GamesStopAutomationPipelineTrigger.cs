using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class GamesStopAutomationPipelineTrigger : IGameAutomationPipelineTrigger
{
    public string DisplayName => Resource.GamesStopAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
    {
        var result = automationEvent is GameAutomationEvent { Started: false };
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new GamesStopAutomationPipelineTrigger();
}
