using System.Linq;
using WindowsDisplayAPI;

namespace LenovoLegionToolkit.Lib.System;

public static class Displays
{
    public static Display[] Get() => Display.GetDisplays().ToArray();
}
