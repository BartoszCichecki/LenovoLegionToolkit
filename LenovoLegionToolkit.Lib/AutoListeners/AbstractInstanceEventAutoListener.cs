using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public abstract class AbstractInstanceEventAutoListener : AbstractWMIAutoListener<(ProcessEventInfoType, int, string)>
{
    private readonly ProcessEventInfoType _type;

    protected AbstractInstanceEventAutoListener(ProcessEventInfoType type, string eventName) : base("ROOT\\CIMV2", eventName)
    {
        _type = type;
    }

    protected override (ProcessEventInfoType, int, string) GetValue(PropertyDataCollection properties)
    {
        // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var processName = properties["ProcessName"].Value?.ToString() ?? string.Empty;
        if (!int.TryParse(properties["ProcessID"].Value?.ToString(), out var processId))
            processId = -1;

        return (_type, processId, Path.GetFileNameWithoutExtension(processName));
        // ReSharper enable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    }

    protected override Task OnChangedAsync((ProcessEventInfoType, int, string) value) => Task.CompletedTask;
}
