using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractLenovoLightingFeature<T> : IFeature<T> where T : struct, Enum, IComparable
{
    private readonly int _dataType;
    private readonly int _id;

    protected AbstractLenovoLightingFeature(int id, int dataType)
    {
        _id = id;
        _dataType = dataType;
    }

    public async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_UTILITY_DATA",
            "GetIfSupportOrVersion",
            new() { { "datatype", _dataType } },
            pdc => Convert.ToInt32(pdc["Data"].Value) > 0).ConfigureAwait(false);

            if (!result)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Data type not supported [feature={GetType().Name}]");

                return false;
            }

            _ = await GetStateAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Supported [feature={GetType().Name}]");

            return true;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported [feature={GetType().Name}]", ex);

            return false;
        }
    }

    public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public async Task<T> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

        var result = await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_METHOD",
            "Get_Lighting_Current_Status",
            new() { { "Lighting_ID", _id } },
            pdc => FromInternal(Convert.ToInt32(pdc["Current_State_Type"].Value))).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public async Task SetStateAsync(T state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_METHOD",
            "Set_Lighting_Current_Status",
            new()
            {
                { "Lighting_ID", _id },
                { "Current_State_Type", ToInternal(state) },
                { "Current_Brightness_Level", 1 }
            }).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }

    protected abstract T FromInternal(int value);

    protected abstract int ToInternal(T state);
}
