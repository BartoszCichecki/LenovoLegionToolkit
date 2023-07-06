using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public class DGPUCapabilityNotify : AbstractDGPUNotify
{
    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi is { Features.Source: MachineInformation.FeatureData.SourceType.CapabilityData, Properties.SupportsIGPUMode: true };
        }
        catch
        {
            return false;
        }
    }

    protected override Task NotifyDGPUStatusAsync(bool state) => SetFeatureValueAsync(CapabilityID.GPUStatus, state ? 1 : 0);

    protected override async Task<HardwareId> GetDGPUHardwareIdAsync()
    {
        try
        {
            var id = await GetFeatureValueAsync(CapabilityID.GPUDidVid).ConfigureAwait(false);
            var vendorId = id & 0xFFFF;
            var deviceId = id >> 16;
            return new HardwareId { Vendor = $"{vendorId:X}", Device = $"{deviceId:X}" };
        }
        catch (Exception)
        {
            return HardwareId.Empty;
        }
    }

    private static Task<int> GetFeatureValueAsync(CapabilityID id) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "GetFeatureValue",
        new() { { "IDs", (int)id } },
        pdc => Convert.ToInt32(pdc["Value"].Value));

    private static Task SetFeatureValueAsync(CapabilityID id, int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "SetFeatureValue",
        new()
        {
            { "IDs", (int)id },
            { "value", value }
        });
}
