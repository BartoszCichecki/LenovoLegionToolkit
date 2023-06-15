using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

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
                return false;

            _ = await GetStateAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

    public Task<T> GetStateAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_LIGHTING_METHOD",
        "Get_Lighting_Current_Status",
        new() { { "Lighting_ID", _id } },
        pdc => FromInternal(Convert.ToInt32(pdc["Current_State_Type"].Value)));

    public Task SetStateAsync(T state) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_LIGHTING_METHOD",
        "Set_Lighting_Current_Status",
        new()
        {
            { "Lighting_ID", _id },
            { "Current_State_Type", ToInternal(state) },
            { "Current_Brightness_Level", 1 }
        });

    protected abstract T FromInternal(int value);

    protected abstract int ToInternal(T state);
}
