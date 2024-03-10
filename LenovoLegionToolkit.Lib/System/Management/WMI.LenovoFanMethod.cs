using System;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System.Management;

public static partial class WMI
{
    public static class LenovoFanMethod
    {
        public static Task FanSetTableAsync(byte[] fanTable) => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_Set_Table",
            new() { { "FanTable", fanTable } });

        public static Task<bool> FanGetFullSpeedAsync() => CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_METHOD",
            "Fan_Get_FullSpeed",
            [],
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
