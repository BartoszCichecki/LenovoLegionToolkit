using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.OverDrive;

public class OverDriveCapabilityFeature : IFeature<OverDriveState>
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features is { Source: MachineInformation.FeatureData.SourceType.CapabilityData, OverDrive: true };
        }
        catch
        {
            return false;
        }
    }

    public Task<OverDriveState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<OverDriveState>());

    public async Task<OverDriveState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var value = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.OverDrive).ConfigureAwait(false);
        var result = (OverDriveState)value;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result}");

        return result;
    }

    public async Task SetStateAsync(OverDriveState state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}...");

        await WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.OverDrive, (int)state).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state}");
    }
}
