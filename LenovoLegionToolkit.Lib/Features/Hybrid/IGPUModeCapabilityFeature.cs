using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeCapabilityFeature : IFeature<IGPUModeState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi is { Features.Source: MachineInformation.FeatureData.SourceType.CapabilityData, Properties.SupportsIGPUMode: true };
        }
        catch
        {
            return false;
        }
    }

    public Task<IGPUModeState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<IGPUModeState>());

    public async Task<IGPUModeState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var value = await GetFeatureValueAsync(CapabilityID.IGPUMode).ConfigureAwait(false);
        var result = (IGPUModeState)value;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result}");

        return result;
    }

    public async Task SetStateAsync(IGPUModeState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}...");

        await SetFeatureValueAsync(CapabilityID.IGPUMode, (int)state).ConfigureAwait(false);
        if (await GetFeatureValueAsync(CapabilityID.IGPUModeChangeStatus).ConfigureAwait(false) == 0)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Set state to {state}, but dGPU check failed.");

            throw new IGPUModeChangeException(state);
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }

    private static Task<int> GetFeatureValueAsync(CapabilityID id) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "GetFeatureValue",
        new() { { "IDs", (int)id } },
        pdc => Convert.ToInt32(pdc["Value"].Value));

    private static Task SetFeatureValueAsync(CapabilityID id, int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "SetFeatureValue",
        new()
        {
            { "IDs", (int)id },
            { "value", value }
        });
}
