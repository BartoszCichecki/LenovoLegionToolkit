using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoGameZoneData
    {
        public static Task<bool> ExistsAsync() => WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_DATA");

        public static Task<int> IsSupportSmartFanAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportSmartFan",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetSmartFanModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetSmartFanMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetSmartFanModeAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetSmartFanMode",
            new() { { "Data", data } });

        public static Task<int> GetIntelligentSubModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetIntelligentSubMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetIntelligentSubModeAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIntelligentSubMode",
            new() { { "Data", data } });

        public static Task<int> IsSupportGSyncAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGSync",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetGSyncStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetGSyncStatus",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetGSyncStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetGSyncStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportIGPUModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportIGPUMode",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetIGPUModeStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetIGPUModeStatus",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetIGPUModeStatusAsync(int mode) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIGPUModeStatus",
            new() { { "mode", mode } });

        public static Task NotifyDGPUStatusAsync(int status) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "NotifyDGPUStatus",
            new() { { "Status", status } });

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

        public static Task<int> IsSupportGpuOCAsync() => CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGpuOC",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> IsSupportDisableTPAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportDisableTP",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetTPStatusStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetTPStatus",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetTPStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetTPStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportDisableWinKeyAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportDisableWinKey",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetWinKeyStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetWinKeyStatus",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetWinKeyStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetWinKeyStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportODAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportOD",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetODStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetODStatus",
            new(),
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetODStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetODStatus",
            new() { { "Data", data } });

        public static Task SetLightControlOwnerAsync(int data) => CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetLightControlOwner",
            new() { { "Data", data } });

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
