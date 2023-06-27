﻿using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.WPF.Settings;

public class DashboardSettings : AbstractSettings<DashboardSettings.DashboardSettingsStore>
{
    public class DashboardSettingsStore
    {
        public bool ShowSensors { get; set; } = true;
        public int SensorsRefreshIntervalSeconds { get; set; } = 2;
        public DashboardGroup[]? Groups { get; set; }
    }

    protected override DashboardSettingsStore Default => new();

    public DashboardSettings() : base("dashboard.json") { }
}