using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
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

        var acAdapter = await GetFeatureValueAsync(CapabilityID.InstantBootAc).ConfigureAwait(false);
        var usbPowerDelivery = await GetFeatureValueAsync(CapabilityID.InstantBootUsbPowerDelivery).ConfigureAwait(false);

        var result = (acAdapter, usbPowerDelivery) switch
        {
            (true, true) => InstantBootState.AcAdapterAndUsbPowerDelivery,
            (true, false) => InstantBootState.AcAdapter,
            (false, true) => InstantBootState.UsbPowerDelivery,
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

        var (acAdapter, usbPowerDelivery) = state switch
        {
            InstantBootState.AcAdapterAndUsbPowerDelivery => (true, true),
            InstantBootState.AcAdapter => (true, false),
            InstantBootState.UsbPowerDelivery => (false, true),
            _ => (false, false)
        };

        await SetFeatureValueAsync(CapabilityID.InstantBootAc, acAdapter).ConfigureAwait(false);
        await SetFeatureValueAsync(CapabilityID.InstantBootUsbPowerDelivery, usbPowerDelivery).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }

    private static Task<bool> GetFeatureValueAsync(CapabilityID id) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", id } },
            pdc => Convert.ToInt32(pdc["Value"].Value) != 0);

    private static Task SetFeatureValueAsync(CapabilityID id, bool value) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", id },
                { "value", value ? 1 : 0 }
            });
}
