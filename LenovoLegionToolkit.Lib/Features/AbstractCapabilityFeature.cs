using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractCapabilityFeature<T>(CapabilityID capabilityID)
    : IFeature<T> where T : struct, Enum, IComparable, IConvertible
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi.Features.Source == MachineInformation.FeatureData.SourceType.CapabilityData && mi.Features[capabilityID];
        }
        catch
        {
            return false;
        }
    }

    public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public async Task<T> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

        var value = await WMI.LenovoOtherMethod.GetFeatureValueAsync(capabilityID).ConfigureAwait(false);
        var result = (T)Enum.ToObject(typeof(T), value);
        if (!Enum.IsDefined(result))
            throw new InvalidOperationException($"Undefined value received: {result} [type={typeof(T)}, feature={GetType().Name}]");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public async Task SetStateAsync(T state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        await WMI.LenovoOtherMethod.SetFeatureValueAsync(capabilityID, Convert.ToInt32(state)).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }
}
