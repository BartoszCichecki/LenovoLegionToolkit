using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public readonly struct SensorSettings
    {
        public int CPUSensorID { get; init; }
        public int GPUSensorID { get; init; }
        public int CPUFanID { get; init; }
        public int GPUFanID { get; init; }
    }

    public readonly struct Sensors
    {
        public SensorData CPU { get; init; }
        public SensorData GPU { get; init; }
    }

    public readonly struct SensorData
    {
        public int CurrentTemperature { get; init; }
        public int MaxTemperature { get; init; }
        public int CurrentFanSpeed { get; init; }
        public int MaxFanSpeed { get; init; }
    }

    public class SensorsController
    {
        private readonly SensorSettings _settings = new()
        {
            CPUSensorID = 3,
            GPUSensorID = 4,
            CPUFanID = 0,
            GPUFanID = 1
        };

        public async Task<Sensors> GetDataAsync()
        {
            return new()
            {
                CPU = new()
                {
                    CurrentTemperature = await GetCurrentTemperatureAsync(_settings.CPUSensorID).ConfigureAwait(false),
                    MaxTemperature = 100,
                    CurrentFanSpeed = await GetCurrentFanSpeedAsync(_settings.CPUFanID).ConfigureAwait(false),
                    MaxFanSpeed = await GetMaxFanSpeedAsync(_settings.CPUSensorID, _settings.CPUFanID).ConfigureAwait(false),
                },
                GPU = new()
                {
                    CurrentTemperature = await GetCurrentTemperatureAsync(_settings.GPUSensorID).ConfigureAwait(false),
                    MaxTemperature = 95,
                    CurrentFanSpeed = await GetCurrentFanSpeedAsync(_settings.GPUFanID).ConfigureAwait(false),
                    MaxFanSpeed = await GetMaxFanSpeedAsync(_settings.GPUSensorID, _settings.GPUFanID).ConfigureAwait(false),
                }
            };
        }

        private static Task<int> GetCurrentTemperatureAsync(int sensorID) =>
            WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_METHOD",
                "Fan_GetCurrentSensorTemperature",
                new() { { "SensorID", sensorID } },
                pdc => Convert.ToInt32(pdc["CurrentSensorTemperature"].Value));

        private static Task<int> GetCurrentFanSpeedAsync(int fanID) =>
            WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_METHOD",
                "Fan_GetCurrentFanSpeed",
                new() { { "FanID", fanID } },
                pdc => Convert.ToInt32(pdc["CurrentFanSpeed"].Value));

        private static async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
        {
            var result = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorID} AND Fan_Id = {fanID}",
                pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
            return result.FirstOrDefault();
        }
    }
}
