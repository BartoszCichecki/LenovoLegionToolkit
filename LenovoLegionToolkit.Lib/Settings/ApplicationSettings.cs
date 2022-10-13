using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings
{

    public class ApplicationSettings : AbstractSettings<ApplicationSettings.ApplicationSettingsStore>
    {
        public class ApplicationSettingsStore
        {
            public Theme Theme { get; set; } = Theme.Dark;
            public RGBColor? AccentColor { get; set; }
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; }
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; }
            public bool DontShowNotifications { get; set; }
            public TemperatureUnit TemperatureUnit { get; set; }
            public List<RefreshRate> ExcludedRefreshRates { get; set; } = new();
            public WarrantyInfo? WarrantyInfo { get; set; }
        }

        protected override string FileName => "settings.json";

        public override ApplicationSettingsStore Default => new();
    }
}
