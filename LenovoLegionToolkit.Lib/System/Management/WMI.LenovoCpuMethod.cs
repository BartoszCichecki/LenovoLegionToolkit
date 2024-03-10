using System;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoCpuMethod
    {
        public static Task<(int longTerm, int shortTerm)> CPUGetDefaultPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Default_PowerLimit",
            [],
            pdc =>
            {
                var longTerm = Convert.ToInt32(pdc["DefaultLongTermPowerlimit"].Value);
                var shortTerm = Convert.ToInt32(pdc["DefaultShortTermPowerlimit"].Value);
                return (longTerm, shortTerm);
            });

        public static Task<(int value, int min, int max, int step)> CPUGetLongTermPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_LongTerm_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentLongTerm_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinLongTerm_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxLongTerm_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return (value, min, max, step);
            });

        public static Task CPUSetLongTermPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Set_LongTerm_PowerLimit",
            new() { { "value", value } });

        public static Task<(int value, int min, int max, int step)> CPUGetShortTermPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_ShortTerm_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentShortTerm_PowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinShortTerm_PowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxShortTerm_PowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);

                return (value, min, max, step);
            });

        public static Task CPUSetShortTermPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Set_ShortTerm_PowerLimit",
            new() { { "value", value } });

        public static Task<(int value, int min, int max, int step, int defaultValue)> CPUGetPeakPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Peak_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentPeakPowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinPeakPowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxPeakPowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultPeakPowerLimit"].Value);

                return (value, min, max, step, defaultValue);
            });

        public static Task CPUSetPeakPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Set_Peak_PowerLimit",
            new() { { "CurrentPeakPowerLimit", value } });

        public static Task<(int value, int min, int max, int step, int defaultValue)> CPUGetCrossLoadingPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Cross_Loading_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentCpuCrossLoading"].Value);
                var min = Convert.ToInt32(pdc["MinCpuCrossLoading"].Value);
                var max = Convert.ToInt32(pdc["MaxCpuCrossLoading"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultCpuCrossLoading"].Value);

                return (value, min, max, step, defaultValue);
            });

        public static Task CPUSetCrossLoadingPowerLimitAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Set_Cross_Loading_PowerLimit",
            new() { { "CurrentCpuCrossLoading", value } });

        public static Task<(int value, int min, int max, int step, int defaultValue)> GetAPUSPPTPowerLimitAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "Get_APU_sPPT_PowerLimit",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrenAPUsPPTPowerLimit"].Value);
                var min = Convert.ToInt32(pdc["MinAPUsPPTPowerLimit"].Value);
                var max = Convert.ToInt32(pdc["MaxAPUsPPTPowerLimit"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultAPUsPPTPowerLimit"].Value);

                return (value, min, max, step, defaultValue);
            });

        public static Task SetAPUSPPTPowerLimitAsync(int value) => WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "Set_APU_sPPT_PowerLimit",
            new() { { "CurrentAPUsPPTPowerLimit", value } });

        public static Task<(int value, int min, int max, int step, int defaultValue)> CPUGetTemperatureControlAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Get_Temperature_Control",
            [],
            pdc =>
            {
                var value = Convert.ToInt32(pdc["CurrentTemperatueControl"].Value);
                var min = Convert.ToInt32(pdc["MinTemperatueControl"].Value);
                var max = Convert.ToInt32(pdc["MaxTemperatueControl"].Value);
                var step = Convert.ToInt32(pdc["step"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultTemperatueControl"].Value);

                return (value, min, max, step, defaultValue);
            });

        public static Task CPUSetTemperatureControlAsync(int value) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CPU_METHOD",
            "CPU_Set_Temperature_Control",
            new() { { "CurrentTemperatureControl", value } });
    }
}
