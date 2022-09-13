using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class DisplayBrightnessController
    {
        public Task<int> GetBrightnessAsync() => WMI.CallAsync(@"root\WMI",
            $"SELECT * FROM WmiMonitorBrightness",
            "WmiGetBrightness",
            new(),
            pdc =>
            {
                return (int)(byte)pdc["CurrentBrightness"].Value;
            });

        public Task SetBrightnessAsync(int brightness) => WMI.CallAsync(@"root\WMI",
            $"SELECT * FROM WmiMonitorBrightnessMethods",
            "WmiSetBrightness",
            new object[] { 1, $"{brightness}" },
            true);
    }
}