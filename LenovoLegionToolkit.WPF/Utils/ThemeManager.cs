using System;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemeManager
    {
        private const string RegistryHive = "HKEY_CURRENT_USER";
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryKey = "AppsUseLightTheme";

#pragma warning disable IDE0052 // Remove unread private members
        private readonly IDisposable _themeListener;
#pragma warning restore IDE0052 // Remove unread private members

        public ThemeManager()
        {
            _themeListener = Registry.Listen(RegistryHive, RegistryPath, RegistryKey, () => Application.Current.Dispatcher.Invoke(() =>
            {
                Apply();
            }));
        }

        public void Apply()
        {
            var theme = Settings.Instance.Theme;
            var registryValue = Registry.Read(RegistryHive, RegistryPath, RegistryKey, 1);

            var currentTheme = (theme, registryValue) switch
            {
                (Theme.Light, _) => WPFUI.Theme.Style.Light,
                (Theme.System, 1) => WPFUI.Theme.Style.Light,
                _ => WPFUI.Theme.Style.Dark,
            };
            WPFUI.Theme.Manager.Switch(currentTheme);

            SetColor();
        }

        private static void SetColor()
        {
            var accentColor = (Color)ColorConverter.ConvertFromString("#F44336");

            Application.Current.Resources["SystemAccentColor"] = accentColor;
            Application.Current.Resources["SystemAccentColorLight2"] = accentColor;
            Application.Current.Resources["SystemAccentColorLight3"] = accentColor;
        }
    }
}
