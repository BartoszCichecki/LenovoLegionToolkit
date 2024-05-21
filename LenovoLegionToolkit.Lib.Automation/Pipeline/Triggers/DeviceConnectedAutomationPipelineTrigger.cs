using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class DeviceConnectedAutomationPipelineTrigger(string[] instanceIds) : IDeviceAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => "When device is connected";

    public string[] InstanceIds { get; } = instanceIds;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not NativeWindowsMessageEvent { Message: NativeWindowsMessage.DeviceConnected, Data: string deviceInstanceId })
            return Task.FromResult(false);

        return Task.FromResult(InstanceIds.Contains(deviceInstanceId));
    }

    public Task<bool> IsMatchingState()
    {
        return Task.FromResult(false);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) { }

    public IAutomationPipelineTrigger DeepCopy() => new DeviceConnectedAutomationPipelineTrigger(InstanceIds);

    public IDeviceAutomationPipelineTrigger DeepCopy(string[] instanceIds) => new DeviceConnectedAutomationPipelineTrigger(instanceIds);

    public override bool Equals(object? obj) => obj is DeviceConnectedAutomationPipelineTrigger t && InstanceIds.SequenceEqual(t.InstanceIds);

    public override int GetHashCode() => InstanceIds.GetHashCode();
}
