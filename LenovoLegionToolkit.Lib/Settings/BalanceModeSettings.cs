using static LenovoLegionToolkit.Lib.Settings.BalanceModeSettings;

namespace LenovoLegionToolkit.Lib.Settings;

public class BalanceModeSettings : AbstractSettings<BalanceModeSettingsStore>
{
    public class BalanceModeSettingsStore
    {
        public bool AIModeEnabled { get; set; }
    }

    public BalanceModeSettings() : base("balancemode.json") { }
}