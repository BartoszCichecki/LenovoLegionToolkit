using static LenovoLegionToolkit.Lib.Settings.GPUOverclockSettings;

namespace LenovoLegionToolkit.Lib.Settings;

public class GPUOverclockSettings() : AbstractSettings<GPUOverclockSettingsStore>("gpu_oc.json")
{
    public class GPUOverclockSettingsStore
    {
        public bool Enabled { get; set; }
        public GPUOverclockInfo Info { get; set; } = GPUOverclockInfo.Zero;
    }
}
