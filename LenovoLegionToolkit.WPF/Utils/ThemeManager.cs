using System;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemeManager
    {
        private const string RegistryHive = "HKEY_CURRENT_USER";
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryKey = "AppsUseLightTheme";

        private readonly IDisposable _themeListener;

        public static bool IsDarkMode
        {
            get
            {
                var theme = Settings.Instance.Theme;
                var registryValue = Registry.Read(RegistryHive, RegistryPath, RegistryKey, 1);

                return (theme, registryValue) switch
                {
                    (Theme.Light, _) => false,
                    (Theme.System, 1) => false,
                    _ => true,
                };
            }
        }

        public event EventHandler? ThemeApplied;

        public ThemeManager()
        {
            _themeListener = Registry.Listen(RegistryHive, RegistryPath, RegistryKey, () => Application.Current.Dispatcher.Invoke(() =>
            {
                Apply();
            }));
        }

        public void Apply()
        {
            SetTheme();
            SetColor();

            ThemeApplied?.Invoke(this, EventArgs.Empty);
        }

        private static void SetTheme()
        {
            var theme = IsDarkMode ? WPFUI.Appearance.ThemeType.Dark : WPFUI.Appearance.ThemeType.Light;
            WPFUI.Appearance.Theme.Apply(theme,
                backgroundEffect: WPFUI.Appearance.BackgroundType.Unknown,
                updateAccent: false);
        }

        private static void SetColor()
        {
            var accentColor = (Color)ColorConverter.ConvertFromString("#E74C3C");
            WPFUI.Appearance.Accent.Apply(systemAccent: accentColor,
                primaryAccent: accentColor,
                secondaryAccent: accentColor,
                tertiaryAccent: accentColor);
        }
    }
}
