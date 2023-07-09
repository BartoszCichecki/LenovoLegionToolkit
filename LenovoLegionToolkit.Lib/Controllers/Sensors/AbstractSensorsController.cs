﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public abstract class AbstractSensorsController : ISensorsController
{
    private readonly struct GPUInfo
    {
        public static readonly GPUInfo Empty = new() { Utilization = -1, CoreClock = -1, MaxCoreClock = -1, MemoryClock = -1, MaxMemoryClock = -1, Temperature = -1, MaxTemperature = -1 };

        public int Utilization { get; init; }
        public int CoreClock { get; init; }
        public int MaxCoreClock { get; init; }
        public int MemoryClock { get; init; }
        public int MaxMemoryClock { get; init; }
        public int Temperature { get; init; }
        public int MaxTemperature { get; init; }
    }

    private readonly SafePerformanceCounter _percentProcessorPerformanceCounter = new("Processor Information", "% Processor Performance", "_Total");
    private readonly SafePerformanceCounter _percentProcessorUtilityCounter = new("Processor Information", "% Processor Utility", "_Total");

    private readonly GPUController _gpuController;

    private int? _cpuBaseClockCache;
    private int? _cpuMaxCoreClockCache;
    private int? _cpuMaxFanSpeedCache;
    private int? _gpuMaxFanSpeedCache;

    protected AbstractSensorsController(GPUController gpuController)
    {
        _gpuController = gpuController ?? throw new ArgumentNullException(nameof(gpuController));
    }

    public abstract Task<bool> IsSupportedAsync();

    public async Task<SensorsData> GetDataAsync()
    {
        const int genericMaxUtilization = 100;
        const int genericMaxTemperature = 100;

        var cpuUtilization = GetCpuUtilization();
        var cpuCoreClock = GetCpuCoreClock();
        var cpuMaxCoreClock = _cpuMaxCoreClockCache ??= await GetCpuMaxCoreClockAsync().ConfigureAwait(false);
        var cpuCurrentTemperature = await GetCpuCurrentTemperatureAsync().ConfigureAwait(false);
        var cpuCurrentFanSpeed = await GetCpuCurrentFanSpeedAsync().ConfigureAwait(false);
        var cpuMaxFanSpeed = _cpuMaxFanSpeedCache ??= await GetCpuMaxFanSpeedAsync().ConfigureAwait(false);

        var gpuInfo = GetGPUInfo();
        var gpuCurrentTemperature = gpuInfo.Temperature >= 0 ? gpuInfo.Temperature : await GetGpuCurrentTemperatureAsync().ConfigureAwait(false);
        var gpuMaxTemperature = gpuInfo.MaxTemperature >= 0 ? gpuInfo.MaxTemperature : genericMaxTemperature;
        var gpuCurrentFanSpeed = await GetGpuCurrentFanSpeedAsync().ConfigureAwait(false);
        var gpuMaxFanSpeed = _gpuMaxFanSpeedCache ??= await GetGpuMaxFanSpeedAsync().ConfigureAwait(false);

        var result = new SensorsData
        {
            CPU = new()
            {
                Utilization = cpuUtilization,
                MaxUtilization = genericMaxUtilization,
                CoreClock = cpuCoreClock,
                MaxCoreClock = cpuMaxCoreClock,
                Temperature = cpuCurrentTemperature,
                MaxTemperature = genericMaxTemperature,
                FanSpeed = cpuCurrentFanSpeed,
                MaxFanSpeed = cpuMaxFanSpeed,
            },
            GPU = new()
            {
                Utilization = gpuInfo.Utilization,
                MaxUtilization = genericMaxUtilization,
                CoreClock = gpuInfo.CoreClock,
                MaxCoreClock = gpuInfo.MaxCoreClock,
                MemoryClock = gpuInfo.MemoryClock,
                MaxMemoryClock = gpuInfo.MaxMemoryClock,
                Temperature = gpuCurrentTemperature,
                MaxTemperature = gpuMaxTemperature,
                FanSpeed = gpuCurrentFanSpeed,
                MaxFanSpeed = gpuMaxFanSpeed,
            }
        };

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current data: {result} [type={GetType().Name}]");

        return result;
    }

    protected abstract Task<int> GetCpuCurrentTemperatureAsync();

    protected abstract Task<int> GetGpuCurrentTemperatureAsync();

    protected abstract Task<int> GetCpuCurrentFanSpeedAsync();

    protected abstract Task<int> GetGpuCurrentFanSpeedAsync();

    protected abstract Task<int> GetCpuMaxFanSpeedAsync();

    protected abstract Task<int> GetGpuMaxFanSpeedAsync();

    private int GetCpuUtilization()
    {
        var result = _percentProcessorUtilityCounter.NextValue();
        return Math.Min(100, result == 0.0 ? -1 : (int)result);
    }

    private int GetCpuCoreClock()
    {
        var baseClock = _cpuBaseClockCache ??= GetCpuBaseClock();
        var clock = baseClock * (_percentProcessorPerformanceCounter.NextValue() / 100.0);
        return clock < 1 ? -1 : (int)clock;
    }

    private static unsafe int GetCpuBaseClock()
    {
        var ptr = IntPtr.Zero;
        try
        {
            PInvoke.GetSystemInfo(out var systemInfo);

            var numberOfProcessors = Math.Min(32, (int)systemInfo.dwNumberOfProcessors);
            var infoSize = Marshal.SizeOf<PROCESSOR_POWER_INFORMATION>();
            var infosSize = numberOfProcessors * infoSize;

            ptr = Marshal.AllocHGlobal(infosSize);

            var result = PInvoke.CallNtPowerInformation(POWER_INFORMATION_LEVEL.ProcessorInformation,
                null,
                0,
                ptr.ToPointer(),
                (uint)infosSize);
            if (result != 0)
                return 0;

            var infos = new PROCESSOR_POWER_INFORMATION[numberOfProcessors];

            for (var i = 0; i < infos.Length; i++)
                infos[i] = Marshal.PtrToStructure<PROCESSOR_POWER_INFORMATION>(IntPtr.Add(ptr, i * infoSize));

            return (int)infos.Select(p => p.MaxMhz).Max();
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static Task<int> GetCpuMaxCoreClockAsync() => WMI.LenovoGameZoneData.GetCPUFrequencyAsync();

    private GPUInfo GetGPUInfo()
    {
        if (_gpuController.LastKnownState is GPUController.GPUState.Inactive or GPUController.GPUState.PoweredOff)
            return GPUInfo.Empty;

        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
                return GPUInfo.Empty;

            var utilization = Math.Min(100, Math.Max(gpu.UsageInformation.GPU.Percentage, gpu.UsageInformation.VideoEngine.Percentage));

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

            return new()
            {
                Utilization = utilization,
                CoreClock = currentCoreClock,
                MaxCoreClock = maxCoreClock + maxCoreClockOffset,
                MemoryClock = currentMemoryClock,
                MaxMemoryClock = maxMemoryClock + maxMemoryClockOffset,
                Temperature = currentTemperature,
                MaxTemperature = maxTemperature
            };
        }
        catch
        {
            return GPUInfo.Empty;
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }
}
