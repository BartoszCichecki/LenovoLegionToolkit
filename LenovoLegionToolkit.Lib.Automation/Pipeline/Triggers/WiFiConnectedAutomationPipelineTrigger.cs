using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class WiFiConnectedAutomationPipelineTrigger : IWiFiConnectedPipelineTrigger
{
    public string DisplayName => "When WiFi is connected";

    public string? Ssid { get; }

    [JsonConstructor]
    public WiFiConnectedAutomationPipelineTrigger(string? ssid)
    {
        Ssid = ssid;
    }

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not WiFiAutomationEvent { IsConnected: true } e)
            return Task.FromResult(false);

        return Task.FromResult(Ssid == null || Ssid == e.Ssid);
    }

    public Task<bool> IsMatchingState()
    {
        var ssid = WiFi.GetConnectedNetworkSSID();

        if (Ssid is null && ssid is not null)
            return Task.FromResult(true);

        return Task.FromResult(Ssid is not null && Ssid == ssid);
    }

    public void UpdateEnvironment(ref AutomationEnvironment environment)
    {
        environment.WiFiConnected = true;
        environment.WiFiSsid = Ssid;
    }

    public IAutomationPipelineTrigger DeepCopy() => new WiFiConnectedAutomationPipelineTrigger(Ssid);

    public IWiFiConnectedPipelineTrigger DeepCopy(string? ssid) => new WiFiConnectedAutomationPipelineTrigger(ssid);

    public override bool Equals(object? obj) => obj is WiFiConnectedAutomationPipelineTrigger t && Ssid == t.Ssid;

    public override int GetHashCode() => HashCode.Combine(Ssid);

    public override string ToString() => $"{nameof(Ssid)}: {Ssid}";
}
