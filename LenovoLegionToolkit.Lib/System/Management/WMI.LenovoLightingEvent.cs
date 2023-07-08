using System;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoLightingEvent
    {
        public static IDisposable Listen(Action<int> handler) => WMI.Listen("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_EVENT",
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Key_ID"].Value);
                handler(value);
            });
    }
}
