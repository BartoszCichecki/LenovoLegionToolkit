using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features;

public abstract class AbstractLenovoLightingFeature<T> : IFeature<PortsBacklightState>
{
    private readonly int _dataType;
    private readonly int _id;

    protected AbstractLenovoLightingFeature(int id, int dataType)
    {
        _id = id;
        _dataType = dataType;
    }

    public Task<bool> IsSupportedAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_UTILITY_DATA",
        "GetIfSupportOrVersion",
        new() { { "datatype", _dataType } },
        pdc => Convert.ToInt32(pdc["Data"].Value) > 0);

    public Task<PortsBacklightState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<PortsBacklightState>());

    public Task<PortsBacklightState> GetStateAsync() => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_LIGHTING_METHOD",
        "Get_Lighting_Current_Status",
        new() { { "Lighting_ID", _id } },
        pdc => (PortsBacklightState)Convert.ToInt32(pdc["Current_State_Type"].Value));

    public Task SetStateAsync(PortsBacklightState state) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_LIGHTING_METHOD",
        "Set_Lighting_Current_Status",
        new()
        {
            { "Lighting_ID", _id },
            { "Current_State_Type", (int)state },
            { "Current_Brightness_Level", 1 }
        });
}
