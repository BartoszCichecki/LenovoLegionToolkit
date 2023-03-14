using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using Newtonsoft.Json;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class DisplayOffAutomationPipelineTrigger : INativeWindowsMessagePipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.DisplayOffAutomationPipelineTrigger_DisplayName;

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        var result = automationEvent is NativeWindowsMessageEvent { Message: NativeWindowsMessage.MonitorOff };
        return Task.FromResult(result);
    }

    public Task<bool> IsMatchingState()
    {
        var result = DisplayAdapter.GetDisplayAdapters()
            .SelectMany(da => da.GetDisplayDevices())
            .Where(dd => dd.IsAvailable)
            .IsEmpty();
        return Task.FromResult(result);
    }

    public IAutomationPipelineTrigger DeepCopy() => new DisplayOffAutomationPipelineTrigger();

    public override bool Equals(object? obj) => obj is DisplayOnAutomationPipelineTrigger;

    public override int GetHashCode() => HashCode.Combine(DisplayName);
}
