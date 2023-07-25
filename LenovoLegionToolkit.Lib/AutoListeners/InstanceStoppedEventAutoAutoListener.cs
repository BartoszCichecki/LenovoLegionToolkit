namespace LenovoLegionToolkit.Lib.AutoListeners;

public class InstanceStoppedEventAutoAutoListener : AbstractInstanceEventAutoListener
{
    public InstanceStoppedEventAutoAutoListener() : base(ProcessEventInfoType.Stopped, "Win32_ProcessStopTrace") { }
}
