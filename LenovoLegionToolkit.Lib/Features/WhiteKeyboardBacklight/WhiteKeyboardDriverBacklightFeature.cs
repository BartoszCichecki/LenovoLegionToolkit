﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features.WhiteKeyboardBacklight;

public class WhiteKeyboardDriverBacklightFeature()
    : AbstractDriverFeature<WhiteKeyboardBacklightState>(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_KEYBOARD)
{
    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var outBuffer = await SendCodeAsync(DriverHandle(), ControlCode, 0x1).ConfigureAwait(false);
            outBuffer >>= 1;
            return outBuffer == 0x2;
        }
        catch
        {
            return false;
        }
    }

    protected override uint GetInBufferValue() => 0x22;

    protected override Task<uint[]> ToInternalAsync(WhiteKeyboardBacklightState state)
    {
        var result = state switch
        {
            WhiteKeyboardBacklightState.Off => new uint[] { 0x00023 },
            WhiteKeyboardBacklightState.Low => [0x10023],
            WhiteKeyboardBacklightState.High => [0x20023],
            _ => throw new InvalidOperationException("Invalid state"),
        };
        return Task.FromResult(result);
    }

    protected override Task<WhiteKeyboardBacklightState> FromInternalAsync(uint state)
    {
        var result = state switch
        {
            0x1 => WhiteKeyboardBacklightState.Off,
            0x3 => WhiteKeyboardBacklightState.Low,
            0x5 => WhiteKeyboardBacklightState.High,
            _ => throw new InvalidOperationException("Invalid state"),
        };
        return Task.FromResult(result);
    }
}
