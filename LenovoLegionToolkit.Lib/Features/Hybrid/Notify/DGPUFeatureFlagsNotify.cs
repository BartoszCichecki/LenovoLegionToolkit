using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
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

    protected override Task NotifyDGPUStatusAsync(bool state) => WMI.LenovoOtherMethod.SetDGPUDeviceStatusAsync(state);

    protected override async Task<HardwareId> GetDGPUHardwareIdAsync()
    {
        try
        {
            return await WMI.LenovoOtherMethod.GetDGPUDeviceDIDVIDAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return HardwareId.Empty;
        }
    }
}
