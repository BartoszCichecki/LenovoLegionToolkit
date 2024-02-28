﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class LowWattageACAdapterConnectedAutomationPipelineTrigger : IPowerStateAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.LowWattageACAdapterConnectedAutomationPipelineTrigger_DisplayName;

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not (PowerStateAutomationEvent or StartupAutomationEvent))
            return false;

        var result = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
        return result == PowerAdapterStatus.ConnectedLowWattage;
    }

    public async Task<bool> IsMatchingState()
    {
        var result = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
        return result == PowerAdapterStatus.ConnectedLowWattage;
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.AcAdapterConnected = true;
        environment.LowPowerAcAdapter = true;
    }

    public IAutomationPipelineTrigger DeepCopy() => new LowWattageACAdapterConnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is LowWattageACAdapterConnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
