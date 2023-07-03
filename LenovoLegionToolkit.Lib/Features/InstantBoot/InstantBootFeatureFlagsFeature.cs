using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootFeatureFlagsFeature : IFeature<InstantBootState>
{
    private const int AC_INDEX = 5;
    private const int USB_POWER_DELIVERY_INDEX = 6;

    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features is { Source: MachineInformation.FeatureData.SourceType.Flags, InstantBootAc: true, InstantBootUsbPowerDelivery: true };
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

        var flags = await GetFlagsAsync().ConfigureAwait(false);

        var acAdapter = flags.IsBitSet(AC_INDEX);
        var usbPowerDelivery = flags.IsBitSet(USB_POWER_DELIVERY_INDEX);

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

        await SetFlagAsync(AC_INDEX, acAdapter).ConfigureAwait(false);
        await SetFlagAsync(USB_POWER_DELIVERY_INDEX, usbPowerDelivery).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }

    private static Task<int> GetFlagsAsync() =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD ",
            "Get_Device_Current_Support_Feature",
            new(),
            pdc => Convert.ToInt32(pdc["Flag"].Value));

    private static Task SetFlagAsync(int flag, bool value) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD ",
            "Set_Device_Current_Support_Feature",
            new()
            {
                { "FunctionID", flag },
                { "value", value ? 1 : 0 }
            });
}
