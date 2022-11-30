using System;
using System.Diagnostics;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.WPF.Utils;

public static class FullscreenHelper
{
    public static bool IsAnyApplicationFullscreen()
    {
        try
        {
            var desktopWindowHandle = PInvoke.GetDesktopWindow();
            var shellWindowHandle = PInvoke.GetShellWindow();

            var foregroundWindowHandle = PInvoke.GetForegroundWindow();
            if (foregroundWindowHandle == HWND.Null)
                return false;
            if (foregroundWindowHandle == desktopWindowHandle)
                return false;
            if (foregroundWindowHandle == shellWindowHandle)
                return false;

            if (!PInvoke.GetWindowRect(foregroundWindowHandle, out var appBounds))
                return false;

            var screenBounds = Screen.FromHandle(foregroundWindowHandle.Value).Bounds;
            var coversFullScreen = appBounds.bottom - appBounds.top == screenBounds.Height && appBounds.right - appBounds.left == screenBounds.Width;
            if (!coversFullScreen)
                return false;

            unsafe
            {
                var processId = 0u;
                _ = PInvoke.GetWindowThreadProcessId(foregroundWindowHandle, &processId);
                var process = Process.GetProcessById((int)processId);
                if (process.ProcessName == "explorer")
                    return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't check if application is full screen.", ex);

            return false;
        }
    }
}