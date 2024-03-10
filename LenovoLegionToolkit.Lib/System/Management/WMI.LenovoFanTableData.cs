using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoFanTableData
    {
        public static Task<bool> ExistsAsync(int sensorId, int fanId) => WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorId} AND Fan_Id = {fanId}");

        public static Task<IEnumerable<(int mode, byte fanId, byte sensorId, ushort[] fanTableData, ushort[] sensorTableData)>> ReadAsync() => WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA",
            pdc =>
            {
                var mode = pdc.Contains("Mode") ? Convert.ToInt32(pdc["Mode"].Value) : -1;
                var fanId = Convert.ToByte(pdc["Fan_Id"].Value);
                var sensorId = Convert.ToByte(pdc["Sensor_ID"].Value);
                var fanTableData = (ushort[]?)pdc["FanTable_Data"].Value ?? [];
                var sensorTableData = (ushort[]?)pdc["SensorTable_Data"].Value ?? [];
                return (mode, fanId, sensorId, fanTableData, sensorTableData);
            });
    }
}
