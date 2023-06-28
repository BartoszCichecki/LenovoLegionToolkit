using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class InstantBootFeature : IFeature<InstantBootState>
{
    private const int AC_ADAPTER_INSTANT_ON_FEATURE = 0x3010001;
    private const int USB_POWER_DELIVERY_INSTANT_ON_FEATURE = 0x3010002;

    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_CAPABILITY_DATA_00 WHERE IDs = {AC_ADAPTER_INSTANT_ON_FEATURE}").ConfigureAwait(false);
            result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_CAPABILITY_DATA_00 WHERE IDs = {USB_POWER_DELIVERY_INSTANT_ON_FEATURE}").ConfigureAwait(false);
            return result;
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

        var acAdapter = await GetFeatureValueAsync(AC_ADAPTER_INSTANT_ON_FEATURE).ConfigureAwait(false);
        var usbPowerDelivery = await GetFeatureValueAsync(USB_POWER_DELIVERY_INSTANT_ON_FEATURE).ConfigureAwait(false);

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

        await SetFeatureValueAsync(AC_ADAPTER_INSTANT_ON_FEATURE, acAdapter).ConfigureAwait(false);
        await SetFeatureValueAsync(USB_POWER_DELIVERY_INSTANT_ON_FEATURE, usbPowerDelivery).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }

    private static Task<bool> GetFeatureValueAsync(int id) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", id } },
            pdc => Convert.ToInt32(pdc["Value"].Value) != 0);

    private static Task SetFeatureValueAsync(int id, bool value) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "SetFeatureValue",
            new()
            {
                { "IDs", id },
                { "value", value ? 1 : 0 }
            });
}
