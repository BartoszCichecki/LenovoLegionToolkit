using System.Linq;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.System;

public static class ExternalDisplays
{
    public static Display[] Get()
    {
        var internalDisplay = InternalDisplay.Get();
        var allDisplays = Display.GetDisplays();
        return allDisplays.Where(d => d.DevicePath != internalDisplay?.DevicePath).ToArray();
    }
}
