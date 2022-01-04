using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemeManager
    {
        public ThemeManager()
        {
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
        }

        public void Apply()
        {
            switch (Settings.Instance.Theme)
            {
                case Theme.Dark:
                    WPFUI.Theme.Manager.Switch(WPFUI.Theme.Style.Dark, true);
                    break;
                case Theme.Light:
                    WPFUI.Theme.Manager.Switch(WPFUI.Theme.Style.Light, true);
                    break;
                case Theme.System:
                    WPFUI.Theme.Manager.SetSystemTheme(true);
                    break;
            }

            SetColor();
        }

        public void ApplyBackgroundEffects(Window window)
        {
            WPFUI.Background.Manager.Apply(window, true);
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WindowGlassColor" && Settings.Instance.Theme == Theme.System)
                Apply();
        }

        private static void SetColor()
        {
            var accentColor = (Color)ColorConverter.ConvertFromString("#f44336");

            Application.Current.Resources["SystemAccentColor"] = accentColor;
            Application.Current.Resources["SystemAccentColorLight2"] = accentColor;
            Application.Current.Resources["SystemAccentColorLight3"] = accentColor;
        }
    }
}
