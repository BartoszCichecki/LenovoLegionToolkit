using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;

namespace LenovoLegionToolkit.WPF.Utils;

public static class ScreenHelper
{
    public static List<ScreenInfo> Screens { get; } = [];

    public static ScreenInfo? PrimaryScreen => Screens.FirstOrDefault(s => s.IsPrimary);

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

#pragma warning disable CA1416
        if (!PInvoke.GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY).Succeeded)
#pragma warning restore CA1416
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
