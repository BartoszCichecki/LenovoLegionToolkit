namespace LenovoLegionToolkit.Lib.AutoListeners;

public class InstanceStartedEventAutoAutoListener : AbstractInstanceEventAutoListener
{
    public InstanceStartedEventAutoAutoListener() : base(ProcessEventInfoType.Started, "Win32_ProcessStartTrace") { }
}
