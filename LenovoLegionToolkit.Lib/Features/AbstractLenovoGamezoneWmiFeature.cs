﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractLenovoGamezoneWmiFeature<T> : IFeature<T> where T : struct, Enum, IComparable
{
    protected static readonly string Scope = @"root\WMI";
    protected static readonly FormattableString Query = $"SELECT * FROM LENOVO_GAMEZONE_DATA";

    private readonly string _methodNameSuffix;
    private readonly int _offset;
    private readonly string? _supportMethodName;
    private readonly int _supportOffset;
    private readonly string _inParameterName;
    private readonly string _outParameterName;

    protected AbstractLenovoGamezoneWmiFeature(string methodNameSuffix,
        int offset,
        string? supportMethodName = null,
        int supportOffset = 0,
        string inParameterName = "Data",
        string outParameterName = "Data")
    {
        _methodNameSuffix = methodNameSuffix;
        _offset = offset;
        _supportMethodName = supportMethodName;
        _supportOffset = supportOffset;
        _inParameterName = inParameterName;
        _outParameterName = outParameterName;
    }

    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            if (_supportMethodName is null)
                return true;

            var value = await WMI.CallAsync(Scope,
                Query,
                _supportMethodName,
                new(),
                pdc => Convert.ToInt32(pdc[_outParameterName].Value)).ConfigureAwait(false);
            return value > _supportOffset;
        }
        catch
        {
            return false;
        }
    }

    public virtual Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public virtual async Task<T> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

        var internalResult = await WMI.CallAsync(Scope,
            Query,
            "Get" + _methodNameSuffix,
            new(),
            pdc => Convert.ToInt32(pdc[_outParameterName].Value)).ConfigureAwait(false);
        var result = FromInternal(internalResult);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public virtual async Task SetStateAsync(T state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        await WMI.CallAsync(Scope,
            Query,
            "Set" + _methodNameSuffix,
            new() { { _inParameterName, ToInternal(state).ToString() } }).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }

    private int ToInternal(T state) => (int)(object)state + _offset;

    private T FromInternal(int state) => (T)(object)(state - _offset);
}