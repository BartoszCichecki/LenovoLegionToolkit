

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoGpuMethod
    {
        public static Task<(int ctgp, int ppab)> GPUGetDefaultPPABcTGPPowerLimit() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_Default_PPAB_cTGP_PowerLimit",
            [],
            pdc =>
            {
                var ctgp = Convert.ToInt32(pdc["Default_cTGP_Powerlimit"].Value);
                var ppab = Convert.ToInt32(pdc["Default_PPAB_Powerlimit"].Value);
                return (ctgp, ppab);
            });

        public static Task<(int value, int min, int max, int step)> GPUGetCTGPPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_cTGP_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Current_cTGP_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["Min_cTGP_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["Max_cTGP_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return (value, min, max, step);
            });

        public static Task GPUSetCTGPPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Set_cTGP_PowerLimit",
            new() { { "value", value } });

        public static Task<(int value, int min, int max, int step)> GPUGetPPABPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_PPAB_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentPPAB_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinPPAB_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxPPAB_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return (value, min, max, step);
            });

        public static Task GPUSetPPABPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Set_PPAB_PowerLimit",
            new() { { "value", value } });

        public static Task<(int value, int min, int max, int step, int defaultValue)> GPUGetTemperatureLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Get_Temperature_Limit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentTemperatueLimit"].Value);
                var min = Convert.ToInt32(pdc["MinTemperatueLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxTemperatueLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultTemperatueLimit"].Value);

                return (value, min, max, step, defaultValue);
            });

        public static Task GPUSetTemperatureLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GPU_METHOD",
            "GPU_Set_Temperature_Limit",
            new() { { "CurrentTemperatureLimit", value } });
    }
}
