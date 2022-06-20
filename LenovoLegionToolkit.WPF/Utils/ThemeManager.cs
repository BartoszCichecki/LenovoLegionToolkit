using System;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemeManager
    {
        private const string RegistryHive = "HKEY_CURRENT_USER";
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryKey = "AppsUseLightTheme";

        private readonly ApplicationSettings _settings;
        private readonly IDisposable _themeListener;

        public bool IsDarkMode
        {
            get
            {
                var theme = _settings.Store.Theme;
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

        public ThemeManager(ApplicationSettings settings)
        {
            _settings = settings;
            _themeListener = Registry.Listen(RegistryHive, RegistryPath, RegistryKey, () => Application.Current.Dispatcher.Invoke(Apply));
        }

        public void Apply()
        {
            SetTheme();
            SetColor();

            ThemeApplied?.Invoke(this, EventArgs.Empty);
        }

        private void SetTheme()
        {
            var theme = IsDarkMode ? Wpf.Ui.Appearance.ThemeType.Dark : Wpf.Ui.Appearance.ThemeType.Light;
            Wpf.Ui.Appearance.Theme.Apply(theme, Wpf.Ui.Appearance.BackgroundType.Mica, false);
        }

        private static void SetColor()
        {
            var accentColor = (Color)ColorConverter.ConvertFromString("#E74C3C");
            Wpf.Ui.Appearance.Accent.Apply(systemAccent: accentColor,
                primaryAccent: accentColor,
                secondaryAccent: accentColor,
                tertiaryAccent: accentColor);
        }
    }
}
