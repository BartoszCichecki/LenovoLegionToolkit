﻿using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Listeners;

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
        var listener = IoCContainer.Resolve<GameListener>();
        var result = !listener.AreGamesRunning();
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(ref AutomationEnvironment environment) => environment.GameRunning = false;

    public IAutomationPipelineTrigger DeepCopy() => new GamesStopAutomationPipelineTrigger();
}
