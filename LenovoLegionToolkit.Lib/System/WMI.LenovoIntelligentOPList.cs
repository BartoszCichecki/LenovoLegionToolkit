﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoIntelligentOPList
    {
        public static async Task<Dictionary<string, int>> ReadAsync()
        {
            var result = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_INTELLIGENT_OP_LIST",
                pdc =>
                {
                    // ReSharper disable once StringLiteralTypo
                    var processName = pdc["processname"].Value.ToString();
                    var mode = Convert.ToInt32(pdc["mode"].Value);
                    return (processName, mode);
                }).ConfigureAwait(false);
            return result.OfType<(string, int)>().ToDictionary(sm => sm.Item1, sm => sm.Item2);
        }
    }
}