using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.WPF.Settings;

public class DashboardSettings : AbstractSettings<DashboardSettings.DashboardSettingsStore>
{
    public class DashboardSettingsStore
    {
        public DashboardGroup[]? Groups { get; set; } = null;
    }

    protected override string FileName => "dashboard.json";

    public override DashboardSettingsStore Default => new();
}