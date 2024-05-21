using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class DeviceAutomationPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<INativeWindowsMessagePipelineTrigger>
{
    private readonly IDeviceAutomationPipelineTrigger _trigger;

    private readonly HashSet<string> _instanceIds;

    public DeviceAutomationPipelineTriggerTabItemContent(IDeviceAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _instanceIds = [.. trigger.InstanceIds];

        InitializeComponent();
    }

    private async void DeviceAutomationPipelineTriggerTabItemContent_Initialized(object? sender, EventArgs e)
    {
        var devices = await Task.Run(Devices.GetAll);
    }

    public INativeWindowsMessagePipelineTrigger GetTrigger() => _trigger.DeepCopy(_instanceIds.ToArray());
}
