using System;
using System.Threading.Tasks;

// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoOtherMethod
    {
        public static Task SetDGPUDeviceStatusAsync(bool state) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "Set_DGPU_Device_Status",
            new() { { "Status", state ? 1 : 0 } });

        public static Task<HardwareId> GetDGPUDeviceDeviceIdVendorId() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "Get_DGPU_Device_DIDVID",
            new(),
            pdc =>
            {
                var id = Convert.ToInt32(pdc["DGPU_ID"].Value);
                var vendorId = id & 0xFFFF;
                var deviceId = id >> 16;
                return new HardwareId { Vendor = $"{vendorId:X}", Device = $"{deviceId:X}" };
            });

        public static Task<int> GetFeatureValueAsync(CapabilityID id) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", (int)id } },
            pdc => Convert.ToInt32(pdc["Value"].Value));

        public static Task SetFeatureValueAsync(CapabilityID id, int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", (int)id },
                { "value", value }
            });
    }
}
