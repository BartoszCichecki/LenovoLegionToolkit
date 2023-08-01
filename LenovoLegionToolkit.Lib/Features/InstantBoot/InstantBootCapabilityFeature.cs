using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootCapabilityFeature : IFeature<InstantBootState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features is { Source: MachineInformation.FeatureData.SourceType.CapabilityData, InstantBootAc: true, InstantBootUsbPowerDelivery: true };
        }
        catch
        {
            return false;
        }
    }

    public Task<InstantBootState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<InstantBootState>());

    public async Task<InstantBootState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var acAdapterValue = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.InstantBootAc).ConfigureAwait(false);
        var usbPowerDeliveryValue = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.InstantBootUsbPowerDelivery).ConfigureAwait(false);

        var result = (acAdapterValue, usbPowerDeliveryValue) switch
        {
            (1, 1) => InstantBootState.AcAdapterAndUsbPowerDelivery,
            (1, 0) => InstantBootState.AcAdapter,
            (0, 1) => InstantBootState.UsbPowerDelivery,
            _ => InstantBootState.Off
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result}");

        return result;
    }

    public async Task SetStateAsync(InstantBootState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}...");

        var (acAdapterValue, usbPowerDeliveryValue) = state switch
        {
            InstantBootState.AcAdapterAndUsbPowerDelivery => (1, 1),
            InstantBootState.AcAdapter => (1, 0),
            InstantBootState.UsbPowerDelivery => (0, 1),
            _ => (0, 0)
        };

        await WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.InstantBootAc, acAdapterValue).ConfigureAwait(false);
        await WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.InstantBootUsbPowerDelivery, usbPowerDeliveryValue).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }
}
