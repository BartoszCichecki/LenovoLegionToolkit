using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers;

public class DisplayBrightnessController
{
    public Task SetBrightnessAsync(int brightness) => WMI.WmiMonitorBrightnessMethods.WmiSetBrightness(brightness, 1);
}
