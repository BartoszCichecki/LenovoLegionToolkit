using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

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

    public async Task<Lib.SensorsData> GetDataAsync()
    {
        var (coreClock, maxCoreClock, memoryClock, maxMemoryClock) = GetGPUClocks();
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
                CoreClock = coreClock,
                MaxCoreClock = maxCoreClock,
                MemoryClock = memoryClock,
                MaxMemoryClock = maxMemoryClock,
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

    private static (int coreClock, int maxCoreClock, int memoryClock, int maxMemoryClock) GetGPUClocks()
    {
        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
                return default;

            var currentCoreClock = (int)gpu.CurrentClockFrequencies.GraphicsClock.Frequency / 1000;
            var currentMemoryClock = (int)gpu.CurrentClockFrequencies.MemoryClock.Frequency / 1000;

            var maxCoreClock = (int)gpu.BaseClockFrequencies.GraphicsClock.Frequency / 1000;
            var maxMemoryClock = (int)gpu.BaseClockFrequencies.MemoryClock.Frequency / 1000;

            var states = GPUApi.GetPerformanceStates20(gpu.Handle);
            var maxCoreClockOffset = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
            var maxMemoryClockOffset = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;

            return (currentCoreClock, maxCoreClock + maxCoreClockOffset, currentMemoryClock, maxMemoryClock + maxMemoryClockOffset);
        }
        catch
        {
            return default;
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }
}
