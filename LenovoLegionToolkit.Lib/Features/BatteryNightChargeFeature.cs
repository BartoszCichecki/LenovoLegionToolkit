using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features;

public class BatteryNightChargeFeature() : AbstractDriverFeature<BatteryNightChargeState>(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_BATTERY_NIGHT_CHARGE)
{
    protected override uint GetInBufferValue() => 0x11;

    protected override Task<uint[]> ToInternalAsync(BatteryNightChargeState state)
    {
        uint[] result = state switch
        {
            BatteryNightChargeState.On => [0x80000012u],
            BatteryNightChargeState.Off => [0x12u],
            _ => throw new InvalidOperationException("Invalid state")
        };
        return Task.FromResult(result);
    }

    protected override Task<BatteryNightChargeState> FromInternalAsync(uint state)
    {
        if (state.GetNthBit(0))
            return Task.FromResult(state.GetNthBit(4) ? BatteryNightChargeState.On : BatteryNightChargeState.Off);

        throw new InvalidOperationException($"Unknown battery night charge state: {state} [bits={Convert.ToString(state, 2)}]");
    }
}
