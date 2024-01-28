namespace LenovoLegionToolkit.Lib.Settings;

public class IntegrationsSettings : AbstractSettings<IntegrationsSettings.IntegrationsSettingsStore>
{
    public class IntegrationsSettingsStore
    {
        public bool HWiNFO { get; set; }
    }

    public IntegrationsSettings() : base("integrations.json") { }
}
