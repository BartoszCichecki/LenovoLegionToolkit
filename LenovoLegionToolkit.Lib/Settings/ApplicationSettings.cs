using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings
{

    public class ApplicationSettings : AbstractSettings<ApplicationSettings.ApplicationSettingsStore>
    {
        public class ApplicationSettingsStore
        {
            public WindowSize WindowSize { get; set; }
            public Theme Theme { get; set; } = Theme.Dark;
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; } = false;
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; } = false;
            public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.C;
        }

        protected override string FileName => "settings.json";
    }
}
