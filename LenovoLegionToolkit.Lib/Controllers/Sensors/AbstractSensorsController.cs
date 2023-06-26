﻿using System;
using System.Diagnostics;
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

    private readonly PerformanceCounter _percentProcessorPerformanceCounter = new("Processor Information", "% Processor Performance", "_Total");
    private readonly PerformanceCounter _percentProcessorUtilityCounter = new("Processor Information", "% Processor Utility", "_Total");

    private int? _cpuBaseClockCache;
    private int? _cpuMaxCoreClockCache;
    private int? _cpuMaxFanSpeedCache;
    private int? _cpuMaxTemperatureCache;
    private int? _gpuMaxFanSpeedCache;

    protected AbstractSensorsController()
    {
        _percentProcessorPerformanceCounter.NextValue();
        _percentProcessorUtilityCounter.NextValue();
    }

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
        var cpuUtilization = GetCpuUtilization();
        var cpuCoreClock = GetCpuCoreClock();
        var cpuMaxCoreClock = _cpuMaxCoreClockCache ??= await GetCpuMaxCoreClockAsync().ConfigureAwait(false);
        var cpuCurrentTemperature = await GetCurrentTemperatureAsync(Settings.CPUSensorID).ConfigureAwait(false);
        var cpuMaxTemperature = _cpuMaxTemperatureCache ??= await GetCpuMaxTemperatureAsync().ConfigureAwait(false);
        var cpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.CPUFanID).ConfigureAwait(false);
        var cpuMaxFanSpeed = _cpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.CPUSensorID, Settings.CPUFanID).ConfigureAwait(false);

        var (gpuUtilization, gpuCoreClock, gpuMaxCoreClock, gpuMemoryClock, gpuMaxMemoryClock, gpuTemperature, gpuMaxTemperature) = GetGPUInfo();
        var gpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.GPUFanID).ConfigureAwait(false);
        var gpuMaxFanSpeed = _gpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.GPUSensorID, Settings.GPUFanID).ConfigureAwait(false);

        return new()
        {
            CPU = new()
            {
                Utilization = cpuUtilization,
                CoreClock = cpuCoreClock,
                MaxCoreClock = cpuMaxCoreClock,
                Temperature = cpuCurrentTemperature,
                MaxTemperature = cpuMaxTemperature,
                FanSpeed = cpuCurrentFanSpeed,
                MaxFanSpeed = cpuMaxFanSpeed,
            },
            GPU = new()
            {
                Utilization = gpuUtilization,
                CoreClock = gpuCoreClock,
                MaxCoreClock = gpuMaxCoreClock,
                MemoryClock = gpuMemoryClock,
                MaxMemoryClock = gpuMaxMemoryClock,
                Temperature = gpuTemperature,
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

    private int GetCpuUtilization() => (int)_percentProcessorUtilityCounter.NextValue();

    private int GetCpuCoreClock()
    {
        var baseClock = _cpuBaseClockCache ??= GetCpuBaseClock();
        var clock = (int)(baseClock * (_percentProcessorPerformanceCounter.NextValue() / 100.0));
        return clock < 1 ? -1 : clock;
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

    private static async Task<int> GetCpuMaxTemperatureAsync()
    {
        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM MSAcpi_ThermalZoneTemperature",
            pdc =>
            {
                var max = Convert.ToInt32(pdc["CriticalTripPoint"].Value);
                max -= 2731;
                max /= 10;
                return max;
            }).ConfigureAwait(false);
        return result.DefaultIfEmpty(-1).FirstOrDefault();
    }

    private static (int utilization, int coreClock, int maxCoreClock, int memoryClock, int maxMemoryClock, int temperature, int maxTemperature) GetGPUInfo()
    {
        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
                return (-1, -1, -1, -1, -1, -1, -1);

            var utilization = Math.Max(gpu.UsageInformation.GPU.Percentage, gpu.UsageInformation.VideoEngine.Percentage);

            var currentCoreClock = (int)gpu.CurrentClockFrequencies.GraphicsClock.Frequency / 1000;
            var currentMemoryClock = (int)gpu.CurrentClockFrequencies.MemoryClock.Frequency / 1000;

            var maxCoreClock = (int)gpu.BoostClockFrequencies.GraphicsClock.Frequency / 1000;
            var maxMemoryClock = (int)gpu.BoostClockFrequencies.MemoryClock.Frequency / 1000;

            var states = GPUApi.GetPerformanceStates20(gpu.Handle);
            var maxCoreClockOffset = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
            var maxMemoryClockOffset = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;

            var temperatureSensor = gpu.ThermalInformation.ThermalSensors.FirstOrDefault();
            var currentTemperature = temperatureSensor?.CurrentTemperature ?? -1;
            var maxTemperature = temperatureSensor?.DefaultMaximumTemperature ?? -1;

            return (utilization, currentCoreClock, maxCoreClock + maxCoreClockOffset, currentMemoryClock, maxMemoryClock + maxMemoryClockOffset, currentTemperature, maxTemperature);
        }
        catch
        {
            return (-1, -1, -1, -1, -1, -1, -1);
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }
}