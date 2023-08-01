using System;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoLightingMethod
    {
        public static Task<(int stateType, int level)> GetLightingCurrentStatusAsync(int lightingId) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_METHOD",
            "Get_Lighting_Current_Status",
            new() { { "Lighting_ID", lightingId } },
            pdc =>
            {
                var stateType = Convert.ToInt32(pdc["Current_State_Type"].Value);
                var level = Convert.ToInt32(pdc["Current_Brightness_Level"].Value);
                return (stateType, level);
            });

        public static Task SetLightingCurrentStatusAsync(int lightingId, int stateType, int level) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_LIGHTING_METHOD",
            "Set_Lighting_Current_Status",
            new()
            {
                { "Lighting_ID", lightingId },
                { "Current_State_Type", stateType },
                { "Current_Brightness_Level", level }
            });
    }
}
