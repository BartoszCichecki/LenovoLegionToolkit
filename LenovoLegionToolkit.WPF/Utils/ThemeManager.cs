using System.ComponentModel;
using System.Windows;
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
                    WPFUI.Theme.Manager.Switch(WPFUI.Theme.Style.Dark);
                    break;
                case Theme.Light:
                    WPFUI.Theme.Manager.Switch(WPFUI.Theme.Style.Light);
                    break;
                case Theme.System:
                    WPFUI.Theme.Manager.SetSystemTheme();
                    break;
            }
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
    }
}
