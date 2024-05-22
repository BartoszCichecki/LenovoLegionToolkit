using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class DeviceDisconnectedAutomationPipelineTrigger(string[] instanceIds) : IDeviceAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.DeviceDisconnectedAutomationPipelineTrigger_DisplayName;

    public string[] InstanceIds { get; } = instanceIds;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not NativeWindowsMessageEvent { Message: NativeWindowsMessage.DeviceDisconnected, Data: string deviceInstanceId })
            return Task.FromResult(false);

        return Task.FromResult(InstanceIds.Contains(deviceInstanceId));
    }

    public Task<bool> IsMatchingState()
    {
        return Task.FromResult(false);
    }

    public void UpdateEnvironment(AutomationEnvironment environment) { }

    public IAutomationPipelineTrigger DeepCopy() => new DeviceDisconnectedAutomationPipelineTrigger(InstanceIds);

    public IDeviceAutomationPipelineTrigger DeepCopy(string[] instanceIds) => new DeviceDisconnectedAutomationPipelineTrigger(instanceIds);

    public override bool Equals(object? obj) => obj is DeviceDisconnectedAutomationPipelineTrigger t && InstanceIds.SequenceEqual(t.InstanceIds);

    public override int GetHashCode() => InstanceIds.GetHashCode();
}
