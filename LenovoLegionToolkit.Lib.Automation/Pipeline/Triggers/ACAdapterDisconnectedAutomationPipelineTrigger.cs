﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class ACAdapterDisconnectedAutomationPipelineTrigger : IPowerStateAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.ACAdapterDisconnectedAutomationPipelineTrigger_DisplayName;

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not (PowerStateAutomationEvent { PowerStateEvent: PowerStateEvent.StatusChange, PowerAdapterStateChanged: true } or StartupAutomationEvent))
            return false;

        var status = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
        return status == PowerAdapterStatus.Disconnected;
    }

    public async Task<bool> IsMatchingState()
    {
        var status = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
        return status == PowerAdapterStatus.Disconnected;
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.AcAdapterConnected = false;

    public IAutomationPipelineTrigger DeepCopy() => new ACAdapterDisconnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is ACAdapterDisconnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
