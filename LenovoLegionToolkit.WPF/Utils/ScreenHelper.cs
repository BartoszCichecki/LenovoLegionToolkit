using System.Runtime.InteropServices;
using System.Windows;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Point = System.Drawing.Point;

#pragma warning disable CA1416 // Validate platform compatibility

namespace LenovoLegionToolkit.WPF.Utils;

public static class ScreenHelper
{
    public static Rect GetPrimaryDesktopWorkingArea()
    {
        var screen = PInvoke.MonitorFromPoint(Point.Empty, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

        var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        if (!PInvoke.GetMonitorInfo(screen, ref monitorInfo))
            return SystemParameters.WorkArea;

        if (!PInvoke.GetDpiForMonitor(screen, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY).Succeeded)
            return SystemParameters.WorkArea;

        var workArea = monitorInfo.rcWork;
        var multiplierX = 96d / dpiX;
        var multiplierY = 96d / dpiY;

        return new Rect(workArea.X, workArea.Y, workArea.Width * multiplierX, workArea.Height * multiplierY);
    }
}
