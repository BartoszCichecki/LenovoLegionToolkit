using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public class DGPUFeatureFlagsNotify : AbstractDGPUNotify
{
    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi is { Features.Source: MachineInformation.FeatureData.SourceType.Flags, Properties.SupportsIGPUMode: true };
        }
        catch
        {
            return false;
        }
    }

    protected override Task NotifyDGPUStatusAsync(bool state) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "Set_DGPU_Device_Status",
        new() { { "Status", state ? 1 : 0 } });

    protected override async Task<HardwareId> GetDGPUHardwareIdAsync()
    {
        try
        {
            return await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_OTHER_METHOD",
                "Get_DGPU_Device_DIDVID",
                new(),
                pdc =>
                {
                    var id = Convert.ToInt32(pdc["DGPU_ID"].Value);
                    var vendorId = id & 0xFFFF;
                    var deviceId = id >> 16;
                    return new HardwareId { Vendor = $"{vendorId:X}", Device = $"{deviceId:X}" };
                }).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return HardwareId.Empty;
        }
    }
}
