using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static partial class LenovoGameZoneData
    {
        [GeneratedRegex("PCIVEN_([0-9A-F]{4})|DEV_([0-9A-F]{4})")]
        private static partial Regex DGPUHWIdRegex();

        public static Task<bool> ExistsAsync() => WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_DATA");

        public static Task<int> IsSupportSmartFanAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportSmartFan",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetSmartFanModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetSmartFanMode",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetSmartFanModeAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetSmartFanMode",
            new() { { "Data", data } });

        public static Task<int> GetIntelligentSubModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetIntelligentSubMode",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetIntelligentSubModeAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetIntelligentSubMode",
            new() { { "Data", data } });

        public static Task<int> IsSupportGSyncAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGSync",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetGSyncStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetGSyncStatus",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetGSyncStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetGSyncStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportIGPUModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportIGPUMode",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetIGPUModeStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetIGPUModeStatus",
            [],
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
            [],
            pdc =>
            {
                var id = pdc["Data"].Value.ToString();

                if (id is null)
                    return HardwareId.Empty;

                try
                {
                    var matches = DGPUHWIdRegex().Matches(id);
                    if (matches.Count != 2)
                        return HardwareId.Empty;

                    var vendor = matches[0].Groups[1].Value;
                    var device = matches[1].Groups[2].Value;

                    return new HardwareId(vendor, device);
                }
                catch
                {
                    return HardwareId.Empty;
                }
            });

        public static Task<int> IsSupportGpuOCAsync() => CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGpuOC",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> IsSupportDisableTPAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportDisableTP",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetTPStatusStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetTPStatus",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetTPStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetTPStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportDisableWinKeyAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportDisableWinKey",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetWinKeyStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetWinKeyStatus",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task SetWinKeyStatusAsync(int data) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "SetWinKeyStatus",
            new() { { "Data", data } });

        public static Task<int> IsSupportODAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportOD",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetODStatusAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetODStatus",
            [],
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
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetPowerChargeModeAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetPowerChargeMode",
            [],
            pdc => Convert.ToInt32(pdc["Data"].Value));

        public static Task<int> GetCPUFrequencyAsync() => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetCPUFrequency",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Data"].Value);
                var low = value & 0xFFFF;
                var high = value >> 16;
                return Math.Max(low, high);
            });
    }
}
