using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV1(GPUController gpuController) : AbstractSensorsController(gpuController)
{
    private const int CPU_SENSOR_ID = 3;
    private const int GPU_SENSOR_ID = 4;
    private const int CPU_FAN_ID = 0;
    private const int GPU_FAN_ID = 1;

    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.LenovoFanTableData.ExistsAsync(0, CPU_FAN_ID).ConfigureAwait(false);
            result &= await WMI.LenovoFanTableData.ExistsAsync(0, GPU_FAN_ID).ConfigureAwait(false);

            if (result)
                _ = await GetDataAsync().ConfigureAwait(false);

            return result;
        }
        catch
        {
            return false;
        }
    }

    protected override async Task<int> GetCpuCurrentTemperatureAsync()
    {
        var t = await WMI.LenovoFanMethod.FanGetCurrentSensorTemperatureAsync(CPU_SENSOR_ID).ConfigureAwait(false);
        if (t < 1)
            return -1;
        return t;
    }

    protected override async Task<int> GetGpuCurrentTemperatureAsync()
    {
        var t = await WMI.LenovoFanMethod.FanGetCurrentSensorTemperatureAsync(GPU_SENSOR_ID).ConfigureAwait(false);
        if (t < 1)
            return -1;
        return t;
    }

    protected override Task<int> GetCpuCurrentFanSpeedAsync() => WMI.LenovoFanMethod.FanGetCurrentFanSpeedAsync(CPU_FAN_ID);

    protected override Task<int> GetGpuCurrentFanSpeedAsync() => WMI.LenovoFanMethod.FanGetCurrentFanSpeedAsync(GPU_FAN_ID);

    protected override Task<int> GetCpuMaxFanSpeedAsync() => WMI.LenovoFanMethod.GetDefaultFanMaxSpeedAsync(0, CPU_FAN_ID);

    protected override Task<int> GetGpuMaxFanSpeedAsync() => WMI.LenovoFanMethod.GetDefaultFanMaxSpeedAsync(0, GPU_FAN_ID);
}
