using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoGameZoneData
    {
        public static Task<bool> Exists() => ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_DATA");

        public static Task<int> IsSupportSmartFanAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportSmartFan",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> IsACFitForOCAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsACFitForOC",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetPowerChargeModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetPowerChargeMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<bool> IsSupportIGPUModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportIGPUMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value) > 0);

        public static Task<bool> IsSupportGpuOCAsync() => CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGpuOC",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value) != 0);

        public static Task NotifyDGPUStatusAsync(bool status) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "NotifyDGPUStatus",
            new() { { "Status", status ? 1 : 0 } });

        public static Task<HardwareId> GetDGPUHWIdAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetDGPUHWId",
            new(),
            pdc =>
            {
                var id = pdc["Data"].Value.ToString();

                if (id is null)
                    return default;

                try
                {
                    var matches = new Regex("PCIVEN_([0-9A-F]{4})|DEV_([0-9A-F]{4})").Matches(id);
                    if (matches.Count != 2)
                        return default;

                    var vendor = matches[0].Groups[1].Value;
                    var device = matches[1].Groups[2].Value;

                    return new HardwareId { Vendor = vendor, Device = device };
                }
                catch
                {
                    return default;
                }
            });

        public static Task<int> GetIntelligentSubModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetIntelligentSubMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetIntelligentSubModeAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIntelligentSubMode",
            new() { { "Data", data } });

        public static Task SetLightControlOwnerAsync(int data) => CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetLightControlOwner",
            new() { { "Data", data } });

        public static Task<int> GetCPUFrequencyAsync() => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetCPUFrequency",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Data"].Value);
                var low = value & 0xFFFF;
                var high = value >> 16;
                return Math.Max(low, high);
            });
    }
}
