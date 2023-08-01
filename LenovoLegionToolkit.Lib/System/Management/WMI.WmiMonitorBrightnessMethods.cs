using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class WmiMonitorBrightnessMethods
    {
        public static Task WmiSetBrightness(int brightness, int timeout) => CallAsync("root\\WMI",
            $"SELECT * FROM WmiMonitorBrightnessMethods",
            "WmiSetBrightness",
            new()
            {
                { "Brightness", brightness },
                { "Timeout", timeout }
            });
    }
}
