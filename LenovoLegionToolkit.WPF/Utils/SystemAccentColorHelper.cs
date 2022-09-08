using LenovoLegionToolkit.Lib.Settings;
using System;
using System.Windows.Media;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class SystemAccentColorHelper
    {
        private const string RegistryHive = "HKEY_CURRENT_USER";
        private const string RegistryPath = @"Software\Microsoft\Windows\DWM";
        private const string RegistryKey = "AccentColor";
        
        private readonly ApplicationSettings _settings;

        private readonly IDisposable _systemAccentColorListener;

        public RGBColor SystemAccentColor => new
                    (((SolidColorBrush)SystemParameters.WindowGlassBrush).Color.R,
                    ((SolidColorBrush)SystemParameters.WindowGlassBrush).Color.G,
                    ((SolidColorBrush)SystemParameters.WindowGlassBrush).Color.B);

        public event EventHandler? AccentColorApplied;
        
        public SystemAccentColorHelper(ApplicationSettings settings)
        {
            _settings = settings;
            _systemAccentColorListener = Registry.Listen(RegistryHive, RegistryPath, RegistryKey, () => Application.Current.Dispatcher.Invoke(Apply));
        }

        public void Apply()
        {
            SetSystemAccentColor();
            AccentColorApplied?.Invoke(this, EventArgs.Empty);
        }

        private void SetSystemAccentColor()
        {
            SetColor(SystemAccentColor);
            _settings.Store.AccentColor = SystemAccentColor;
            _settings.SynchronizeStore();
        }

        private void SetColor(RGBColor color)
        {
            var accentColorRgb = color;
            var accentColor = Color.FromRgb(accentColorRgb.R, accentColorRgb.G, accentColorRgb.B);
            Wpf.Ui.Appearance.Accent.Apply(systemAccent: accentColor,
                primaryAccent: accentColor,
                secondaryAccent: accentColor,
                tertiaryAccent: accentColor);
        }
    }
}
