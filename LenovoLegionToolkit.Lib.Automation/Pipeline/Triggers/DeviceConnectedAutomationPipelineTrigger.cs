using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

[method: JsonConstructor]
public class DeviceConnectedAutomationPipelineTrigger(string[]? instanceIds) : IDeviceAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.DeviceConnectedAutomationPipelineTrigger_DisplayName;

    public string[] InstanceIds { get; } = instanceIds ?? [];

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not NativeWindowsMessageEvent { Message: NativeWindowsMessage.DeviceConnected, Data: string deviceInstanceId })
            return Task.FromResult(false);

        return Task.FromResult(InstanceIds.Contains(deviceInstanceId));
    }

    public Task<bool> IsMatchingState()
    {
        var result = Devices.GetAll()
            .Where(d => !d.IsDisconnected)
            .Select(d => d.DeviceInstanceId)
            .Intersect(InstanceIds)
            .Any();
        return Task.FromResult(result);
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.DeviceConnected = true;
        environment.DeviceInstanceIds = InstanceIds;
    }

    public IAutomationPipelineTrigger DeepCopy() => new DeviceConnectedAutomationPipelineTrigger(InstanceIds);

    public IDeviceAutomationPipelineTrigger DeepCopy(string[] instanceIds) => new DeviceConnectedAutomationPipelineTrigger(instanceIds);

    public override bool Equals(object? obj) => obj is DeviceConnectedAutomationPipelineTrigger t && InstanceIds.SequenceEqual(t.InstanceIds);

    public override int GetHashCode()
    {
        var hc = new HashCode();
        InstanceIds.ForEach(id => hc.Add(id));
        return hc.ToHashCode();
    }

    public override string ToString() => $"{nameof(InstanceIds)}: {string.Join(", ", InstanceIds)}";
}
