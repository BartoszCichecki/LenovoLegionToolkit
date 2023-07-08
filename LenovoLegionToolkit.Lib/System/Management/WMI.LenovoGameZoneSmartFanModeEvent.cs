using System;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoGameZoneSmartFanModeEvent
    {
        public static IDisposable Listen(Action<int> handler) => WMI.Listen("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT",
            pdc =>
            {
                var value = Convert.ToInt32(pdc["mode"].Value);
                handler(value);
            });
    }
}
