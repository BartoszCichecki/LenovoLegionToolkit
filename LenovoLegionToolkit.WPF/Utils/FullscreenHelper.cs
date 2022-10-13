using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class FullscreenHelper
    {
        public static bool IsAnyApplicationFullscreen()
        {
            var hwndDesktop = PInvoke.GetDesktopWindow();
            var hwndShell = PInvoke.GetShellWindow();

            var hwndForeground = PInvoke.GetForegroundWindow();
            if (hwndForeground == HWND.Null || hwndForeground == hwndDesktop || hwndForeground == hwndShell)
                return false;

            var result = PInvoke.GetWindowRect(hwndForeground, out var appBounds);
            if (!result)
                return false;

            var screenBounds = Screen.FromHandle(hwndForeground.Value).Bounds;

            return appBounds.bottom - appBounds.top == screenBounds.Height && appBounds.right - appBounds.left == screenBounds.Width;
        }
    }
}
