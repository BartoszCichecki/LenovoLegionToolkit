using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using NotImplementedException = System.NotImplementedException;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class GamesStopAutomationPipelineTrigger : IGameAutomationPipelineTrigger
{
    public string DisplayName => Resource.GamesStopAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is GameAutomationEvent { Started: false };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        throw new NotImplementedException();
    }

    public IAutomationPipelineTrigger DeepCopy() => new GamesStopAutomationPipelineTrigger();
}
