using LenovoLegionToolkit.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Point = System.Drawing.Point;


#pragma warning disable CA1416

namespace LenovoLegionToolkit.WPF.Utils;

public static class ScreenHelper
{
    public static List<ScreenInfo> Screens { get; private set; } = [];

    public static ScreenInfo PrimaryScreen => Screens.Where(s => s.IsPrimary).First();

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

    public static void UpdateScreenInfos()
    {
        Screens.Clear();
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumDisplayMonitorsDelegate lpfnEnum, IntPtr dwData);

    private delegate bool EnumDisplayMonitorsDelegate(HMONITOR hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    private static bool MonitorEnumProc(HMONITOR hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
    {
        MONITORINFO monitorInfo = new() { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };

        if (!PInvoke.GetMonitorInfo(hMonitor, ref monitorInfo))
            return true;

        if (!PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY).Succeeded)
            return true;

        var workArea = monitorInfo.rcWork;
        var multiplierX = 96d / dpiX;
        var multiplierY = 96d / dpiY;

        Screens.Add(new ScreenInfo(
            new Rect(workArea.X, workArea.Y, workArea.Width * multiplierX, workArea.Height * multiplierY),
            dpiX, dpiY,
            (monitorInfo.dwFlags & PInvoke.MONITORINFOF_PRIMARY) != 0
        ));

        return true;
    }
}
