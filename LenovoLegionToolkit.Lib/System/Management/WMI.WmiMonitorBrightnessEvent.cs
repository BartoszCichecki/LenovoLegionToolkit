using System;

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class WmiMonitorBrightnessEvent
    {
        public static IDisposable Listen(Action<byte> handler) => WMI.Listen("root\\WMI",
            $"SELECT * FROM WmiMonitorBrightnessEvent",
            pdc =>
            {
                var value = Convert.ToByte(pdc["Brightness"].Value);
                handler(value);
            });
    }
}