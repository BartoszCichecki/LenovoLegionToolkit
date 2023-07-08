using System;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoUtilityEvent
    {
        public static IDisposable Listen(Action<int> handler) => WMI.Listen("root\\WMI",
            $"SELECT * FROM LENOVO_UTILITY_EVENT",
            pdc =>
            {
                var value = Convert.ToInt32(pdc["PressTypeDataVal"].Value);
                handler(value);
            });
    }
}
