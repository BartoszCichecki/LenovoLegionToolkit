using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

[method: JsonConstructor]
public class WiFiConnectedAutomationPipelineTrigger(string[] ssids) : IWiFiConnectedPipelineTrigger
{
    public string DisplayName => Resource.WiFiConnectedAutomationPipelineTrigger_DisplayName;

    public string[] Ssids { get; } = ssids;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not WiFiAutomationEvent { IsConnected: true } e)
            return Task.FromResult(false);

        return Task.FromResult(Ssids.IsEmpty() || Ssids.Contains(e.Ssid));
    }

    public Task<bool> IsMatchingState()
    {
        var ssid = WiFi.GetConnectedNetworkSsid();

        if (Ssids.IsEmpty() && ssid is not null)
            return Task.FromResult(true);

        return Task.FromResult(Ssids.Contains(ssid));
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.WiFiConnected = true;
        environment.WiFiSsid = string.Join(",", Ssids);
    }

    public IAutomationPipelineTrigger DeepCopy() => new WiFiConnectedAutomationPipelineTrigger(Ssids);

    public IWiFiConnectedPipelineTrigger DeepCopy(string[] ssids) => new WiFiConnectedAutomationPipelineTrigger(ssids);

    public override bool Equals(object? obj) => obj is WiFiConnectedAutomationPipelineTrigger t && Ssids.SequenceEqual(t.Ssids);

    public override int GetHashCode() => HashCode.Combine(Ssids);

    public override string ToString() => $"{nameof(Ssids)}: {string.Join(",", Ssids)}";
}
