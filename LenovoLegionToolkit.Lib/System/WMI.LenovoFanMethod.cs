using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static partial class WMI
{
    public static class LenovoFanMethod
    {
        public static Task<bool> ExistsAsync(int sensorId, int fanId) => WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorId} AND Fan_Id = {fanId}");

        public static Task<IEnumerable<(int mode, byte fanId, byte sensorId, ushort[] fanTableData, ushort[] sensorTableData)>> FanGetTable() => ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA",
            pdc =>
            {
                var mode = Convert.ToInt32(pdc["Mode"].Value);
                var fanId = Convert.ToByte(pdc["Fan_Id"].Value);
                var sensorId = Convert.ToByte(pdc["Sensor_ID"].Value);
                var fanTableData = (ushort[]?)pdc["FanTable_Data"].Value ?? Array.Empty<ushort>();
                var sensorTableData = (ushort[]?)pdc["SensorTable_Data"].Value ?? Array.Empty<ushort>();
                return (mode, fanId, sensorId, fanTableData, sensorTableData);
            });

        public static Task FanSetTable(byte[] fanTable) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_Set_Table",
            new() { { "FanTable", fanTable } });

        public static Task<bool> FanGetFullSpeedAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_Get_FullSpeed",
            new(),
            pdc => (bool)pdc["Status"].Value);

        public static Task FanSetFullSpeedAsync(int status) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_Set_FullSpeed",
            new() { { "Status", status } });

        public static Task<int> FanGetCurrentSensorTemperatureAsync(int sensorId) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_GetCurrentSensorTemperature",
            new() { { "SensorID", sensorId } },
            pdc => Convert.ToInt32(pdc["CurrentSensorTemperature"].Value));

        public static Task<int> FanGetCurrentFanSpeedAsync(int fanId) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_GetCurrentFanSpeed",
            new() { { "FanID", fanId } },
            pdc => Convert.ToInt32(pdc["CurrentFanSpeed"].Value));

        public static async Task<int> GetCurrentFanMaxSpeedAsync(int sensorId, int fanId)
        {
            var result = await ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorId} AND Fan_Id = {fanId}",
                pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
            return result.Max();
        }

        public static async Task<int> GetDefaultFanMaxSpeedAsync(int sensorId, int fanID)
        {
            var result = await ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorId} AND Fan_Id = {fanID}",
                pdc => Convert.ToInt32(pdc["DefaultFanMaxSpeed"].Value)).ConfigureAwait(false);
            return result.Max();
        }
    }
}
