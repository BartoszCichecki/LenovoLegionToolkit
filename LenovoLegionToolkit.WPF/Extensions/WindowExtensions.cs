using System.Windows;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class WindowExtensions
    {
        public static void BringToForeground(this Window window)
        {
            window.ShowInTaskbar = true;

            if (window.WindowState == WindowState.Minimized || window.Visibility == Visibility.Hidden)
            {
                window.Show();
                window.WindowState = WindowState.Normal;
            }

            window.Activate();
            window.Topmost = true;
            window.Topmost = false;
            window.Focus();
        }
    }
}
