using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.FlipToStart;

public class FlipToStartCapabilityFeature : IFeature<FlipToStartState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features is { Source: MachineInformation.FeatureData.SourceType.CapabilityData, FlipToStart: true };
        }
        catch
        {
            return false;
        }
    }

    public Task<FlipToStartState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<FlipToStartState>());

    public async Task<FlipToStartState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var value = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.FlipToStart).ConfigureAwait(false);
        var result = (FlipToStartState)value;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result}");

        return result;
    }

    public async Task SetStateAsync(FlipToStartState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}...");

        await WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.FlipToStart, (int)state).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }
}
