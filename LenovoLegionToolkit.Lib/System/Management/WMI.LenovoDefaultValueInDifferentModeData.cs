using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoDefaultValueInDifferentModeData
    {
        public readonly struct Data(
            int mode,
            int cpuLongTermPowerLimit,
            int cpuShortTermPowerLimit,
            int cpuPeakPowerLimit,
            int cpuCrossLoadingPowerLimit,
            int apUsPPTPowerLimit,
            int cpuTemperatureLimit,
            int gpuPowerBoost,
            int gpuConfigurableTGP,
            int gpuTemperatureLimit)
        {
            public int Mode { get; } = mode;
            public int CPULongTermPowerLimit { get; } = cpuLongTermPowerLimit;
            public int CPUShortTermPowerLimit { get; } = cpuShortTermPowerLimit;
            public int CPUPeakPowerLimit { get; } = cpuPeakPowerLimit;
            public int CPUCrossLoadingPowerLimit { get; } = cpuCrossLoadingPowerLimit;
            public int APUsPPTPowerLimit { get; } = apUsPPTPowerLimit;
            public int CPUTemperatureLimit { get; } = cpuTemperatureLimit;
            public int GPUPowerBoost { get; } = gpuPowerBoost;
            public int GPUConfigurableTGP { get; } = gpuConfigurableTGP;
            public int GPUTemperatureLimit { get; } = gpuTemperatureLimit;
        }

        public static Task<IEnumerable<Data>> ReadAsync() => WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_DEFAULT_VALUE_IN_DIFFERENT_MODE_DATA ",
            pdc => new Data(Convert.ToInt32(pdc["mode"].Value),
                Convert.ToInt32(pdc["DefaultLongTermPowerlimit"].Value),
                Convert.ToInt32(pdc["DefaultShortTermPowerlimit"].Value),
                Convert.ToInt32(pdc["DefaultPeakPowerLimit"].Value),
                Convert.ToInt32(pdc["DefaultCpuCrossLoading"].Value),
                Convert.ToInt32(pdc["DefaultAPUsPPTPowerLimit"].Value),
                Convert.ToInt32(pdc["DefaultTemperatueControl"].Value),
                Convert.ToInt32(pdc["Default_PPAB_Powerlimit"].Value),
                Convert.ToInt32(pdc["Default_cTGP_Powerlimit"].Value),
                Convert.ToInt32(pdc["DefaultTemperatueLimit"].Value)));
    }
}
