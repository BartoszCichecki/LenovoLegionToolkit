using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoCapabilityData00
    {
        public static Task<IEnumerable<CapabilityID>> ReadAsync() => WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_CAPABILITY_DATA_00",
            pdc => (CapabilityID)Convert.ToInt32(pdc["IDs"].Value));
    }
}
