﻿using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.Settings
{

    public class ApplicationSettings : AbstractSettings<ApplicationSettings.ApplicationSettingsStore>
    {
        public class ApplicationSettingsStore
        {
            public WindowSize WindowSize { get; set; }
            public Theme Theme { get; set; }
            public RGBColor? AccentColor { get; set; }
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
            public bool MinimizeOnClose { get; set; }
            public bool ActivatePowerProfilesWithVantageEnabled { get; set; }
            public TemperatureUnit TemperatureUnit { get; set; }
        }

        protected override string FileName => "settings.json";

        public override ApplicationSettingsStore Default => new()
        {
            Theme = Theme.Dark,
            TemperatureUnit = TemperatureUnit.C,
        };
    }
}
