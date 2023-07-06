using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeFeatureFlagsFeature : IFeature<IGPUModeState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi is { Features.Source: MachineInformation.FeatureData.SourceType.Flags, Properties.SupportsIGPUMode: true };
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

        var flags = await GetFlagsAsync().ConfigureAwait(false);

        var result = IGPUModeState.Default;

        if (flags.IsBitSet(0))
            result = IGPUModeState.IGPUOnly;
        if (flags.IsBitSet(1))
            result = IGPUModeState.Auto;


        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result}");

        return result;
    }

    public async Task SetStateAsync(IGPUModeState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}...");

        var result = await SetFlagAsync(1, (int)state).ConfigureAwait(false);
        if (result == 0)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Set state to {state}, but dGPU check failed.");

            throw new IGPUModeChangeException(state);
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }

    private static Task<int> GetFlagsAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "Get_Device_Current_Support_Feature",
        new(),
        pdc => Convert.ToInt32(pdc["Flag"].Value));

    private static Task<int> SetFlagAsync(int flag, int value) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "Set_Device_Current_Support_Feature",
        new()
        {
            { "FunctionID", flag },
            { "value", value }
        },
        pdc => Convert.ToInt32(pdc["ret"].Value));
}
