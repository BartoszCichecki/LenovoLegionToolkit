using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.InstantBoot;

public class InstantBootCapabilityFeature : IFeature<InstantBootState>
{
    private enum State
    {
        Off,
        On
    }

    private class InstantBootAcFeature() : AbstractCapabilityFeature<State>(CapabilityID.InstantBootAc);
    private class InstantBootUsbPowerDeliveryFeature() : AbstractCapabilityFeature<State>(CapabilityID.InstantBootUsbPowerDelivery);

    private readonly InstantBootAcFeature _ac = new();
    private readonly InstantBootUsbPowerDeliveryFeature _usbPowerDelivery = new();

    public async Task<bool> IsSupportedAsync()
    {
        return await _ac.IsSupportedAsync().ConfigureAwait(false) && await _usbPowerDelivery.IsSupportedAsync().ConfigureAwait(false);
    }

    public Task<InstantBootState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<InstantBootState>());

    public async Task<InstantBootState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var ac = await _ac.GetStateAsync().ConfigureAwait(false);
        var usbPowerDelivery = await _usbPowerDelivery.GetStateAsync().ConfigureAwait(false);

        var result = (ac, usbPowerDelivery) switch
        {
            (State.On, State.On) => InstantBootState.AcAdapterAndUsbPowerDelivery,
            (State.On, State.Off) => InstantBootState.AcAdapter,
            (State.Off, State.On) => InstantBootState.UsbPowerDelivery,
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

        var (ac, usbPowerDelivery) = state switch
        {
            InstantBootState.AcAdapterAndUsbPowerDelivery => (State.On, State.On),
            InstantBootState.AcAdapter => (State.On, State.Off),
            InstantBootState.UsbPowerDelivery => (State.Off, State.On),
            _ => (State.Off, State.Off)
        };

        await _ac.SetStateAsync(ac).ConfigureAwait(false);
        await _usbPowerDelivery.SetStateAsync(usbPowerDelivery).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }
}
