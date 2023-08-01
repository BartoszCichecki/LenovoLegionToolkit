using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractWmiFeature<T> : IFeature<T> where T : struct, Enum, IComparable
{
    private readonly Func<Task<int>> _getValue;
    private readonly Func<int, Task> _setValue;
    private readonly Func<Task<int>>? _isSupported;
    private readonly int _offset;

    protected AbstractWmiFeature(Func<Task<int>> getValue, Func<int, Task> setValue, Func<Task<int>>? isSupported = null, int offset = 0)
    {
        _getValue = getValue;
        _setValue = setValue;
        _isSupported = isSupported;
        _offset = offset;
    }

    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            if (_isSupported is null)
                return true;

            return await _isSupported().ConfigureAwait(false) > 0;
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

        var internalResult = await _getValue().ConfigureAwait(false);
        var result = FromInternal(internalResult);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public virtual async Task SetStateAsync(T state)
    {

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        await _setValue(ToInternal(state)).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }

    private int ToInternal(T state) => (int)(object)state + _offset;

    private T FromInternal(int state) => (T)(object)(state - _offset);
}
