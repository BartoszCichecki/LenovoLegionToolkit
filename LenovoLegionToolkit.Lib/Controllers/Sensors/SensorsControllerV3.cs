using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV3 : AbstractSensorsController
{
    private const int CPU_SENSOR_ID = 4;
    private const int GPU_SENSOR_ID = 5;
    private const int CPU_FAN_ID = 1;
    private const int GPU_FAN_ID = 2;
    private const int CPU_CURRENT_TEMPERATURE_FEATURE_ID = 0x5040000;
    private const int GPU_CURRENT_TEMPERATURE_FEATURE_ID = 0x5050000;
    private const int CPU_CURRENT_FAN_SPEED_FEATURE_ID = 0x4030001;
    private const int GPU_CURRENT_FAN_SPEED_FEATURE_ID = 0x4030002;

    public SensorsControllerV3(GPUController gpuController) : base(gpuController) { }

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

    protected override Task<int> GetCpuCurrentTemperatureAsync() => GetFeatureValueAsync(CPU_CURRENT_TEMPERATURE_FEATURE_ID);

    protected override Task<int> GetGpuCurrentTemperatureAsync() => GetFeatureValueAsync(GPU_CURRENT_TEMPERATURE_FEATURE_ID);

    protected override Task<int> GetCpuCurrentFanSpeedAsync() => GetFeatureValueAsync(CPU_CURRENT_FAN_SPEED_FEATURE_ID);

    protected override Task<int> GetGpuCurrentFanSpeedAsync() => GetFeatureValueAsync(GPU_CURRENT_FAN_SPEED_FEATURE_ID);

    protected override Task<int> GetCpuMaxFanSpeedAsync() => GetMaxFanSpeedAsync(CPU_SENSOR_ID, CPU_FAN_ID);

    protected override Task<int> GetGpuMaxFanSpeedAsync() => GetMaxFanSpeedAsync(GPU_SENSOR_ID, GPU_FAN_ID);

    private static Task<int> GetFeatureValueAsync(int id) =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_OTHER_METHOD",
            "GetFeatureValue",
            new() { { "IDs", id } },
            pdc => Convert.ToInt32(pdc["Value"].Value));

    private static async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
    {
        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorID} AND Fan_Id = {fanID}",
            pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
        return result.Max();
    }
}
