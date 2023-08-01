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
        public readonly struct Data
        {
            public int Mode { get; init; }
            public int CPULongTermPowerLimit { get; init; }
            public int CPUShortTermPowerLimit { get; init; }
            public int CPUPeakPowerLimit { get; init; }
            public int CPUCrossLoadingPowerLimit { get; init; }
            public int APUsPPTPowerLimit { get; init; }
            public int CPUTemperatureLimit { get; init; }
            public int GPUPowerBoost { get; init; }
            public int GPUConfigurableTGP { get; init; }
            public int GPUTemperatureLimit { get; init; }
        }

        public static Task<IEnumerable<Data>> ReadAsync() => WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_DEFAULT_VALUE_IN_DIFFERENT_MODE_DATA ",
            pdc => new Data
            {
                Mode = Convert.ToInt32(pdc["mode"].Value),
                CPULongTermPowerLimit = Convert.ToInt32(pdc["DefaultLongTermPowerlimit"].Value),
                CPUShortTermPowerLimit = Convert.ToInt32(pdc["DefaultShortTermPowerlimit"].Value),
                CPUPeakPowerLimit = Convert.ToInt32(pdc["DefaultPeakPowerLimit"].Value),
                CPUCrossLoadingPowerLimit = Convert.ToInt32(pdc["DefaultCpuCrossLoading"].Value),
                APUsPPTPowerLimit = Convert.ToInt32(pdc["DefaultAPUsPPTPowerLimit"].Value),
                CPUTemperatureLimit = Convert.ToInt32(pdc["DefaultTemperatueControl"].Value),
                GPUPowerBoost = Convert.ToInt32(pdc["Default_PPAB_Powerlimit"].Value),
                GPUConfigurableTGP = Convert.ToInt32(pdc["Default_cTGP_Powerlimit"].Value),
                GPUTemperatureLimit = Convert.ToInt32(pdc["DefaultTemperatueLimit"].Value),
            });
    }
}
