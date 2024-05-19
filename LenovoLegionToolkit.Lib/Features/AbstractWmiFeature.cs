using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractWmiFeature<T>(Func<Task<int>> getValue, Func<int, Task> setValue, Func<Task<int>>? isSupported = null, int offset = 0)
    : IFeature<T> where T : struct, Enum, IComparable
{
    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            if (isSupported is null)
                return true;

            return await isSupported().ConfigureAwait(false) > 0;
        }
        catch
        {
            return false;
        }
    }
    public virtual Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public async Task<T> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

        var internalResult = await getValue().ConfigureAwait(false);
        var result = FromInternal(internalResult);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public virtual async Task SetStateAsync(T state)
    {

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        await setValue(ToInternal(state)).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }

    private int ToInternal(T state) => (int)(object)state + offset;

    private T FromInternal(int state) => (T)(object)(state - offset);
}
