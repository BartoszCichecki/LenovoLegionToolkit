using System;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
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
        private readonly SystemThemeListener _listener = IoCContainer.Resolve<SystemThemeListener>();

        public RGBColor DefaultAccentColor => new(231, 76, 60);

        public bool IsDarkMode
        {
            get
            {
                var theme = _settings.Store.Theme;
                var systemDarkMode = _settings.Default.Theme == Theme.Dark;

                try { systemDarkMode = SystemTheme.GetDarkMode(); } catch { }

                return theme == Theme.Dark || theme == Theme.System && systemDarkMode;
            }
        }

        public event EventHandler? ThemeApplied;

        public ThemeManager(ApplicationSettings settings)
        {
            _settings = settings;
            _listener.Changed += (object? s, SystemThemeSettings e) => Application.Current.Dispatcher.Invoke(Apply);
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

        private void SetColor()
        {
            var accentColorRgb = _settings.Store.AccentColor ?? DefaultAccentColor;
            var accentColor = Color.FromRgb(accentColorRgb.R, accentColorRgb.G, accentColorRgb.B);
            Wpf.Ui.Appearance.Accent.Apply(systemAccent: accentColor,
                primaryAccent: accentColor,
                secondaryAccent: accentColor,
                tertiaryAccent: accentColor);
        }
    }
}
