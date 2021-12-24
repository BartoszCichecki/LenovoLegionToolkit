using System.ComponentModel;
using System.Windows;
using LenovoLegionToolkit.Lib;
using WPFUI.Theme;
using Style = WPFUI.Theme.Style;

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
                    Manager.Switch(Style.Dark);
                    break;
                case Theme.Light:
                    Manager.Switch(Style.Light);
                    break;
                case Theme.System:
                    Manager.SetSystemTheme();
                    break;
            }
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WindowGlassColor" && Settings.Instance.Theme == Theme.System)
                Apply();
        }
    }
}
