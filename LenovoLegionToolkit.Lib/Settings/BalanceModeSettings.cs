using static LenovoLegionToolkit.Lib.Settings.BalanceModeSettings;

namespace LenovoLegionToolkit.Lib.Settings;

public class BalanceModeSettings() : AbstractSettings<BalanceModeSettingsStore>("balancemode.json")
{
    public class BalanceModeSettingsStore
    {
        public bool AIModeEnabled { get; set; }
    }

    // ReSharper disable once StringLiteralTypo
}
