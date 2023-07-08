using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoCapabilityData01
    {
        public static Task<IEnumerable<RangeCapability>> ReadAsync() => WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CAPABILITY_DATA_01",
            pdc =>
            {
                var id = Convert.ToInt32(pdc["IDs"].Value);
                var defaultValue = Convert.ToInt32(pdc["DefaultValue"].Value);
                var min = Convert.ToInt32(pdc["MinValue"].Value);
                var max = Convert.ToInt32(pdc["MaxValue"].Value);
                var step = Convert.ToInt32(pdc["Step"].Value);
                return new RangeCapability((CapabilityID)id, defaultValue, min, max, step);
            });
    }
}
