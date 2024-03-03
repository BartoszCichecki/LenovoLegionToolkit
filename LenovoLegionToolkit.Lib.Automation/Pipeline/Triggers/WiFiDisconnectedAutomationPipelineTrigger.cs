using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class WiFiDisconnectedAutomationPipelineTrigger : IWiFiDisconnectedPipelineTrigger
{
    public string DisplayName => Resource.WiFiDisconnectedAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is WiFiAutomationEvent { IsConnected: false };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var ssid = WiFi.GetConnectedNetworkSsid();
        return Task.FromResult(ssid is null);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) => environment.WiFiConnected = false;

    public IAutomationPipelineTrigger DeepCopy() => new WiFiDisconnectedAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is WiFiDisconnectedAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
