using System;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoGameZoneKeyLockStatusEvent
    {
        public static IDisposable Listen(Action<int> handler) => WMI.Listen("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_KEYLOCK_STATUS_EVENT",
            pdc =>
            {
                var value = Convert.ToInt32(pdc["KeyLockState"].Value);
                handler(value);
            });
    }
}
