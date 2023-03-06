using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.Lib.Automation.Listeners;

internal class InstanceEventListener : AbstractWMIListener<(ProcessEventInfoType, int, string)>
{
    private readonly ProcessEventInfoType _type;

    public InstanceEventListener(ProcessEventInfoType type, string eventName)
        : base("ROOT\\CIMV2", query: $"SELECT * FROM {eventName}")
    {
        _type = type;
    }

    protected override (ProcessEventInfoType, int, string) GetValue(PropertyDataCollection properties)
    {
        var processName = properties["ProcessName"].Value?.ToString() ?? string.Empty;
        if (!int.TryParse(properties["ProcessID"].Value?.ToString(), out int processID))
            processID = -1;

        return (_type, processID, processName);
    }

    protected override Task OnChangedAsync((ProcessEventInfoType, int, string) value) => Task.CompletedTask;
}
