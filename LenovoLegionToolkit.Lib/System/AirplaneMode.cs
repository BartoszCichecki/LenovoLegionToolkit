using System.Diagnostics;

namespace LenovoLegionToolkit.Lib.System;

public static class AirplaneMode
{
    public static void Open()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = "/c \"start ms-settings:network-airplanemode\"",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        });
    }
}
