using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
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

    protected override Task NotifyDGPUStatusAsync(bool state) => WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.GPUStatus, state ? 1 : 0);

    protected override async Task<HardwareId> GetDGPUHardwareIdAsync()
    {
        try
        {
            var value = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.GPUDidVid).ConfigureAwait(false);
            var vendorId = value & 0xFFFF;
            var deviceId = value >> 16;
            return new HardwareId($"{vendorId:X}", $"{deviceId:X}");
        }
        catch (Exception)
        {
            return HardwareId.Empty;
        }
    }
}
