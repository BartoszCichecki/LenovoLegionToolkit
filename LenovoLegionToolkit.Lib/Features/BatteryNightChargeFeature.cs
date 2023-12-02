using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class BatteryNightChargeFeature : AbstractDriverFeature<BatteryNightChargeState>
{
    public BatteryNightChargeFeature() : base(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_BATTERY_NIGHT_CHARGE) { }

    protected override uint GetInBufferValue() => 0x11;

    protected override Task<uint[]> ToInternalAsync(BatteryNightChargeState state)
    {
        var result = state switch
        {
            BatteryNightChargeState.On => new[] { 0x80000012u },
            BatteryNightChargeState.Off => new[] { 0x12u },
            _ => throw new InvalidOperationException("Invalid state.")
        };
        return Task.FromResult(result);
    }

    protected override Task<BatteryNightChargeState> FromInternalAsync(uint state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Internal value: [bits={Convert.ToString(state, 2)}]");

        state = state.ReverseEndianness();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Internal value #2: [bits={Convert.ToString(state, 2)}]");

        if (state.GetNthBit(31))
            return Task.FromResult(BatteryNightChargeState.On);

        if (state.GetNthBit(30))
            return Task.FromResult(BatteryNightChargeState.Off);

        throw new InvalidOperationException($"Unknown battery night charge state: {state} [bits={Convert.ToString(state, 2)}]");
    }
}
