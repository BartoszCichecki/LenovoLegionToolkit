﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features;

public class AlwaysOnUSBFeature() : AbstractDriverFeature<AlwaysOnUSBState>(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_SETTINGS)
{
    protected override uint GetInBufferValue() => 0x2;

    protected override Task<uint[]> ToInternalAsync(AlwaysOnUSBState state)
    {
        var result = state switch
        {
            AlwaysOnUSBState.Off => new uint[] { 0xB, 0x12 },
            AlwaysOnUSBState.OnWhenSleeping => [0xA, 0x12],
            AlwaysOnUSBState.OnAlways => [0xA, 0x13],
            _ => throw new InvalidOperationException("Invalid state"),
        };
        return Task.FromResult(result);
    }

    protected override Task<AlwaysOnUSBState> FromInternalAsync(uint state)
    {
        state = state.ReverseEndianness();

        if (state.GetNthBit(31)) // is on?
        {
            if (state.GetNthBit(23))
                return Task.FromResult(AlwaysOnUSBState.OnAlways);

            return Task.FromResult(AlwaysOnUSBState.OnWhenSleeping);
        }

        return Task.FromResult(AlwaysOnUSBState.Off);
    }
}
