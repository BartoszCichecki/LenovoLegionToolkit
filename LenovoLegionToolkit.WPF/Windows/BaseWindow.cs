using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows
{
    public class BaseWindow : Window
    {
        private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

        public BaseWindow()
        {
            IsVisibleChanged += BaseWindow_IsVisibleChanged;
            DpiChanged += BaseWindow_DpiChanged;
        }

        private void BaseWindow_DpiChanged(object sender, DpiChangedEventArgs e) => VisualTreeHelper.SetRootDpi(this, e.NewDpi);

        private void BaseWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            var result = ApplyBackgroundEffect();
            if (!result)
                return;

            _themeManager.ThemeApplied += ThemeManager_ThemeApplied;

            Closed += BaseWindow_Closed;
            IsVisibleChanged -= BaseWindow_IsVisibleChanged;
        }

        private void BaseWindow_Closed(object? sender, EventArgs e) => _themeManager.ThemeApplied -= ThemeManager_ThemeApplied;

        private void ThemeManager_ThemeApplied(object? sender, EventArgs e) => ApplyBackgroundEffect();

        private bool ApplyBackgroundEffect()
        {
            var ptr = new WindowInteropHelper(this).Handle;
            if (ptr == IntPtr.Zero)
                return false;

            if (!Wpf.Ui.Appearance.Background.IsSupported(Wpf.Ui.Appearance.BackgroundType.Mica))
                return false;

            var result = Wpf.Ui.Appearance.Background.Apply(ptr, Wpf.Ui.Appearance.BackgroundType.Mica);
            if (!result)
                return false;

            Background = Brushes.Transparent;
            return true;
        }
    }
}
