using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoLightingData
    {
        public static Task<bool> ExistsAsync(int lightingId, int controlInterface, int type) =>
            WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_LIGHTING_DATA WHERE Lighting_ID = {lightingId} AND Control_Interface = {controlInterface} AND Lighting_Type = {type}");
    }
}
