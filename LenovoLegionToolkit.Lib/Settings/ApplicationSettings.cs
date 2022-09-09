using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings
{

    public class ApplicationSettings : AbstractSettings<ApplicationSettings.ApplicationSettingsStore>
    {
        public class ApplicationSettingsStore
        {
            public WindowSize WindowSize { get; set; }
            public Theme Theme { get; set; }
            public RGBColor? AccentColor { get; set; }
            public bool SyncSystemAccentColor { get; set; }
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; }
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; }
            public bool DontShowNotifications { get; set; }
            public TemperatureUnit TemperatureUnit { get; set; }
            public List<RefreshRate> ExcludedRefreshRates { get; set; } = new();
        }

        protected override string FileName => "settings.json";

        public override ApplicationSettingsStore Default => new()
        {
            Theme = Theme.System,
            SyncSystemAccentColor = true,
            TemperatureUnit = TemperatureUnit.C,
        };
    }
}
