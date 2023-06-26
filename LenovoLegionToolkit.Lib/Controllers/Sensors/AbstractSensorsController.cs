using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public abstract class AbstractSensorsController : ISensorsController
{
    protected abstract SensorSettings Settings { get; }

    private int? _cpuBaseClockCache;
    private int? _cpuMaxCoreClockCache;
    private int? _cpuMaxFanSpeedCache;
    private int? _gpuMaxFanSpeedCache;

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

    public async Task<SensorsData> GetDataAsync()
    {
        const int cpuMaxTemperature = 100;
        const int gpuMaxTemperature = 95;

        var cpuCoreClock = await GetCpuCoreClockAsync().ConfigureAwait(false);
        var cpuMaxCoreClock = _cpuMaxCoreClockCache ??= await GetCpuMaxCoreClockAsync().ConfigureAwait(false);
        var cpuCurrentTemperature = await GetCurrentTemperatureAsync(Settings.CPUSensorID).ConfigureAwait(false);
        var cpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.CPUFanID).ConfigureAwait(false);
        var cpuMaxFanSpeed = _cpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.CPUSensorID, Settings.CPUFanID).ConfigureAwait(false);

        var (gpuCoreClock, gpuMaxCoreClock, gpuMemoryClock, gpuMaxMemoryClock) = GetGPUClocks();
        var gpuCurrentTemperature = await GetCurrentTemperatureAsync(Settings.GPUSensorID).ConfigureAwait(false);
        var gpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.GPUFanID).ConfigureAwait(false);
        var gpuMaxFanSpeed = _gpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.GPUSensorID, Settings.GPUFanID).ConfigureAwait(false);

        return new()
        {
            CPU = new()
            {
                CoreClock = cpuCoreClock,
                MaxCoreClock = cpuMaxCoreClock,
                Temperature = cpuCurrentTemperature,
                MaxTemperature = cpuMaxTemperature,
                FanSpeed = cpuCurrentFanSpeed,
                MaxFanSpeed = cpuMaxFanSpeed,
            },
            GPU = new()
            {
                CoreClock = gpuCoreClock,
                MaxCoreClock = gpuMaxCoreClock,
                MemoryClock = gpuMemoryClock,
                MaxMemoryClock = gpuMaxMemoryClock,
                Temperature = gpuCurrentTemperature,
                MaxTemperature = gpuMaxTemperature,
                FanSpeed = gpuCurrentFanSpeed,
                MaxFanSpeed = gpuMaxFanSpeed,
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
            $"SELECT CurrentFanMaxSpeed FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {sensorID} AND Fan_Id = {fanID}",
            pdc => Convert.ToInt32(pdc["CurrentFanMaxSpeed"].Value)).ConfigureAwait(false);
        return result.FirstOrDefault();
    }

    private async Task<int> GetCpuCoreClockAsync()
    {
        var baseClock = _cpuBaseClockCache ??= GetCpuBaseClock();
        var percentPerformance = await WMI.ReadAsync("root\\CIMV2",
            $"SELECT Name,PercentProcessorPerformance FROM Win32_PerfFormattedData_Counters_ProcessorInformation WHERE Name = '_Total'",
            pdc => Convert.ToInt32(pdc["PercentProcessorPerformance"].Value)).ConfigureAwait(false);
        return (int)(baseClock * (percentPerformance.First() / 100.0));
    }

    private static unsafe int GetCpuBaseClock()
    {
        var ptr = IntPtr.Zero;
        try
        {
            PInvoke.GetSystemInfo(out var systemInfo);

            var size = Marshal.SizeOf<PROCESSOR_POWER_INFORMATION>() * (int)systemInfo.dwNumberOfProcessors;
            ptr = Marshal.AllocHGlobal(size);

            var result = PInvoke.CallNtPowerInformation(POWER_INFORMATION_LEVEL.ProcessorInformation, null, 0, ptr.ToPointer(), (uint)size);
            if (result != 0)
                return 0;

            var ppi = Marshal.PtrToStructure<PROCESSOR_POWER_INFORMATION>(ptr);
            return (int)ppi.MaxMhz;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static Task<int> GetCpuMaxCoreClockAsync() =>
        WMI.CallAsync("root\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "GetCPUFrequency",
            new(),
            pdc =>
            {
                var value = Convert.ToInt32(pdc["Data"].Value);
                var low = value & 0xFFFF;
                var high = value >> 16;
                return Math.Max(low, high);
            });

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
