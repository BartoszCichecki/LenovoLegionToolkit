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
        private readonly ApplicationSettings _settings;
        private readonly SystemThemeListener _listener = IoCContainer.Resolve<SystemThemeListener>();

        public RGBColor DefaultAccentColor => new(255, 33, 33);

        public bool IsDarkMode
        {
            get
            {
                var theme = _settings.Store.Theme;
                var registryValue = SystemTheme.GetDarkMode();

                return (theme, registryValue) switch
                {
                    (Theme.Light, _) => false,
                    (Theme.System, false) => false,
                    _ => true,
                };
            }
        }

        public RGBColor AccentColor => _settings.Store.AccentColorSource switch
        {
            AccentColorSource.System => SystemTheme.GetThemeMatchedAccentColor(),
            AccentColorSource.Custom => _settings.Store.AccentColor ?? DefaultAccentColor,
            _ => DefaultAccentColor
        };

        public event EventHandler? ThemeApplied;

        public ThemeManager(ApplicationSettings settings)
        {
            _settings = settings;
            _listener.Changed += (_, _) => Application.Current.Dispatcher.Invoke(Apply);
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
            var accentColor = Color.FromRgb(AccentColor.R, AccentColor.G, AccentColor.B);
            Wpf.Ui.Appearance.Accent.Apply(systemAccent: accentColor,
                primaryAccent: accentColor,
                secondaryAccent: accentColor,
                tertiaryAccent: accentColor);
        }
    }
}
