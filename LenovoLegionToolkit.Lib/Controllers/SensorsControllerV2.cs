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

    public interface ISensorsController
    {
        Task<bool> IsSupportedAsync();
        Task<Sensors> GetDataAsync();
    }

    public abstract class AbstractSensorsController : ISensorsController
    {
        protected abstract SensorSettings Settings { get; }

        public virtual async Task<bool> IsSupportedAsync()
        {
            try
            {
                var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {Settings.CPUSensorID} AND Fan_Id = {Settings.CPUFanID}").ConfigureAwait(false);
                result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {Settings.GPUSensorID} AND Fan_Id = {Settings.GPUFanID}").ConfigureAwait(false);
                return result;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Sensors> GetDataAsync()
        {
            return new()
            {
                CPU = new()
                {
                    CurrentTemperature = await GetCurrentTemperatureAsync(Settings.CPUSensorID).ConfigureAwait(false),
                    MaxTemperature = 100,
                    CurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.CPUFanID).ConfigureAwait(false),
                    MaxFanSpeed = await GetMaxFanSpeedAsync(Settings.CPUSensorID, Settings.CPUFanID).ConfigureAwait(false),
                },
                GPU = new()
                {
                    CurrentTemperature = await GetCurrentTemperatureAsync(Settings.GPUSensorID).ConfigureAwait(false),
                    MaxTemperature = 95,
                    CurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.GPUFanID).ConfigureAwait(false),
                    MaxFanSpeed = await GetMaxFanSpeedAsync(Settings.GPUSensorID, Settings.GPUFanID).ConfigureAwait(false),
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

        protected virtual async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
        {
            var result = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorID} AND Fan_Id = {fanID}",
                pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
            return result.FirstOrDefault();
        }
    }

    public class SensorsControllerV1 : AbstractSensorsController
    {
        protected override SensorSettings Settings => new()
        {
            CPUSensorID = 3,
            GPUSensorID = 4,
            CPUFanID = 0,
            GPUFanID = 1
        };

        public override async Task<bool> IsSupportedAsync()
        {
            try
            {
                var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {Settings.CPUFanID}").ConfigureAwait(false);
                result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {Settings.GPUFanID}").ConfigureAwait(false);
                return result;
            }
            catch
            {
                return false;
            }
        }

        protected override async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
        {
            var result = await WMI.ReadAsync("root\\WMI",
                $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {fanID}",
                pdc => Convert.ToInt32(pdc["DefaultFanMaxSpeed"].Value)).ConfigureAwait(false);
            return result.FirstOrDefault();
        }
    }

    public class SensorsControllerV2 : AbstractSensorsController
    {
        protected override SensorSettings Settings => new()
        {
            CPUSensorID = 3,
            GPUSensorID = 4,
            CPUFanID = 0,
            GPUFanID = 1
        };
    }

    public class SensorsControllerV3 : AbstractSensorsController
    {
        protected override SensorSettings Settings => new()
        {
            CPUSensorID = 4,
            GPUSensorID = 5,
            CPUFanID = 1,
            GPUFanID = 2
        };
    }
}
