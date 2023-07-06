using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV2 : AbstractSensorsController
{
    private const int CPU_SENSOR_ID = 3;
    private const int GPU_SENSOR_ID = 4;
    private const int CPU_FAN_ID = 0;
    private const int GPU_FAN_ID = 1;

    public SensorsControllerV2(GPUController gpuController) : base(gpuController) { }

    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {CPU_SENSOR_ID} AND Fan_Id = {CPU_FAN_ID}").ConfigureAwait(false);
            result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {GPU_SENSOR_ID} AND Fan_Id = {GPU_FAN_ID}").ConfigureAwait(false);

            if (result)
                _ = await GetDataAsync().ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Error checking support. [type={GetType().Name}]", ex);

            return false;
        }
    }

    protected override async Task<int> GetCpuCurrentTemperatureAsync()
    {
        var t = await GetCurrentTemperatureAsync(CPU_SENSOR_ID).ConfigureAwait(false);
        if (t < 1)
            return -1;
        return t;
    }

    protected override async Task<int> GetGpuCurrentTemperatureAsync()
    {
        var t = await GetCurrentTemperatureAsync(GPU_SENSOR_ID).ConfigureAwait(false);
        if (t < 1)
            return -1;
        return t;
    }

    protected override Task<int> GetCpuCurrentFanSpeedAsync() => GetCurrentFanSpeedAsync(CPU_FAN_ID);

    protected override Task<int> GetGpuCurrentFanSpeedAsync() => GetCurrentFanSpeedAsync(GPU_FAN_ID);

    protected override Task<int> GetCpuMaxFanSpeedAsync() => GetMaxFanSpeedAsync(CPU_SENSOR_ID, CPU_FAN_ID);

    protected override Task<int> GetGpuMaxFanSpeedAsync() => GetMaxFanSpeedAsync(GPU_SENSOR_ID, GPU_FAN_ID);

    private static Task<int> GetCurrentTemperatureAsync(int sensorID) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_GetCurrentSensorTemperature",
        new() { { "SensorID", sensorID } },
        pdc => Convert.ToInt32(pdc["CurrentSensorTemperature"].Value));

    private static Task<int> GetCurrentFanSpeedAsync(int fanID) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_FAN_METHOD",
        "Fan_GetCurrentFanSpeed",
        new() { { "FanID", fanID } },
        pdc => Convert.ToInt32(pdc["CurrentFanSpeed"].Value));

    private static async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
    {
        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorID} AND Fan_Id = {fanID}",
            pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
        return result.Max();
    }
}
