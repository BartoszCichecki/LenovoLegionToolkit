using LenovoLegionToolkit.Lib.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

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