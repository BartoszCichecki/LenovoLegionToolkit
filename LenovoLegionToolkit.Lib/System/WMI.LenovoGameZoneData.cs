using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoGamezoneData
    {
        public static Task<bool> IsSupportGpuOCAsync() => WMI.CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGpuOC",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value) != 0);

        public static Task NotifyDGPUStatusAsync(bool status) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "NotifyDGPUStatus",
            new() { { "Status", status ? 1 : 0 } });
    }
}
