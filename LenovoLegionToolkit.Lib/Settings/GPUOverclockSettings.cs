using static LenovoLegionToolkit.Lib.Settings.GPUOverclockSettings;

namespace LenovoLegionToolkit.Lib.Settings;

public class GPUOverclockSettings : AbstractSettings<GPUOverclockSettingsStore>
{
    public class GPUOverclockSettingsStore
    {
        public GPUOverclockInfo? PerformanceModeGpuOverclockInfo { get; set; }
    }

    public GPUOverclockSettings() : base("gpuoc.json") { }
}
