using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class FullscreenHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out Rect rc);

        public static bool IsAnyApplicationFullscreen()
        {
            var hwndDesktop = GetDesktopWindow();
            var hwndShell = GetShellWindow();

            var hwndForeground = GetForegroundWindow();
            if (hwndForeground == IntPtr.Zero || hwndForeground == hwndDesktop || hwndForeground == hwndShell)
                return false;

            _ = GetWindowRect(hwndForeground, out Rect appBounds);
            var screenBounds = Screen.FromHandle(hwndForeground).Bounds;

            return appBounds.Bottom - appBounds.Top == screenBounds.Height && appBounds.Right - appBounds.Left == screenBounds.Width;
        }
    }
}
