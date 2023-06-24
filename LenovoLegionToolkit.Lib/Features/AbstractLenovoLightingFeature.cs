using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractLenovoLightingFeature<T> : IFeature<T> where T : struct, Enum, IComparable
{
    private readonly int _lightingID;
    private readonly int _controlInterface;
    private readonly int _type;

    public bool ForceDisable { get; set; }

    protected AbstractLenovoLightingFeature(int lightingID, int controlInterface, int type)
    {
        _lightingID = lightingID;
        _controlInterface = controlInterface;
        _type = type;
    }

    public virtual async Task<bool> IsSupportedAsync()
    {
        if (ForceDisable)
            return false;

        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            if (mi.Properties.IsExcludedFromLenovoLighting)
                return false;

            var isSupported = await WMI.ExistsAsync(
                "root\\WMI",
                $"SELECT * FROM LENOVO_LIGHTING_DATA WHERE Lighting_ID = {_lightingID} AND Control_Interface = {_controlInterface} AND Lighting_Type = {_type}"
                ).ConfigureAwait(false);

            if (!isSupported)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Control interface not found [feature={GetType().Name}]");
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
                Log.Instance.Trace($"Failed to check support [feature={GetType().Name}]", ex);

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
            new() { { "Lighting_ID", _lightingID } },
            pdc =>
            {
                var stateType = Convert.ToInt32(pdc["Current_State_Type"].Value);
                var level = Convert.ToInt32(pdc["Current_Brightness_Level"].Value);
                return FromInternal(stateType, level);
            }).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

        return result;
    }

    public async Task SetStateAsync(T state)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

        var (stateType, level) = ToInternal(state);

        await WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_METHOD",
            "Set_Lighting_Current_Status",
            new()
            {
                { "Lighting_ID", _lightingID },
                { "Current_State_Type",  stateType},
                { "Current_Brightness_Level", level }
            }).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
    }

    protected abstract T FromInternal(int stateType, int level);

    protected abstract (int stateType, int level) ToInternal(T state);
}
