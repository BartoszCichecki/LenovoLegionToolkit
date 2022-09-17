using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class DisplayBrightnessController
    {
        public Task SetBrightnessAsync(int brightness) => WMI.CallAsync(@"root\WMI",
            $"SELECT * FROM WmiMonitorBrightnessMethods",
            "WmiSetBrightness",
            new() { { "Timeout", (uint)1 }, { "Brightness", (byte)brightness } });
    }
}