using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class SingleStepWhiteKeyboardBacklightFeature : AbstractDriverFeature<WhiteKeyboardBacklightState>
{
    public SingleStepWhiteKeyboardBacklightFeature() : base(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_KEYBOARD)
    {
    }

    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var outBuffer = await SendCodeAsync(DriverHandle(), 0x831020E8, 0x2).ConfigureAwait(false);
            var result = ((int)outBuffer & 16) == 16;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"TEST: result={result}");

            return result;
        }
        catch
        {
            return false;
        }
    }

    protected override uint GetInBufferValue() => 0x12;

    protected override Task<uint[]> ToInternalAsync(WhiteKeyboardBacklightState state)
    {
        var result = state switch
        {
            WhiteKeyboardBacklightState.Off => new uint[] { 0x00023 },
            WhiteKeyboardBacklightState.Low => new uint[] { 0x10023 },
            WhiteKeyboardBacklightState.High => new uint[] { 0x20023 },
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