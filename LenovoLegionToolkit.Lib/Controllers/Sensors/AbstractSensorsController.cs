using System;
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

public abstract class AbstractSensorsController(GPUController gpuController) : ISensorsController
{
    private readonly struct GPUInfo(
        int utilization,
        int coreClock,
        int maxCoreClock,
        int memoryClock,
        int maxMemoryClock,
        int temperature,
        int maxTemperature)
    {
        public static readonly GPUInfo Empty = new(-1, -1, -1, -1, -1, -1, -1);

        public int Utilization { get; } = utilization;
        public int CoreClock { get; } = coreClock;
        public int MaxCoreClock { get; } = maxCoreClock;
        public int MemoryClock { get; } = memoryClock;
        public int MaxMemoryClock { get; } = maxMemoryClock;
        public int Temperature { get; } = temperature;
        public int MaxTemperature { get; } = maxTemperature;
    }

    private readonly SafePerformanceCounter _percentProcessorPerformanceCounter = new("Processor Information", "% Processor Performance", "_Total");
    private readonly SafePerformanceCounter _percentProcessorUtilityCounter = new("Processor Information", "% Processor Utility", "_Total");

    private int? _cpuBaseClockCache;
    private int? _cpuMaxCoreClockCache;
    private int? _cpuMaxFanSpeedCache;
    private int? _gpuMaxFanSpeedCache;

    public abstract Task<bool> IsSupportedAsync();

    public Task PrepareAsync()
    {
        _percentProcessorPerformanceCounter.Reset();
        _percentProcessorUtilityCounter.Reset();
        return Task.CompletedTask;
    }

    public async Task<SensorsData> GetDataAsync()
    {
        const int genericMaxUtilization = 100;
        const int genericMaxTemperature = 100;

        var cpuUtilization = GetCpuUtilization(genericMaxUtilization);
        var cpuMaxCoreClock = _cpuMaxCoreClockCache ??= await GetCpuMaxCoreClockAsync().ConfigureAwait(false);
        var cpuCoreClock = GetCpuCoreClock();
        var cpuCurrentTemperature = await GetCpuCurrentTemperatureAsync().ConfigureAwait(false);
        var cpuCurrentFanSpeed = await GetCpuCurrentFanSpeedAsync().ConfigureAwait(false);
        var cpuMaxFanSpeed = _cpuMaxFanSpeedCache ??= await GetCpuMaxFanSpeedAsync().ConfigureAwait(false);

        var gpuInfo = await GetGPUInfoAsync().ConfigureAwait(false);
        var gpuCurrentTemperature = gpuInfo.Temperature >= 0 ? gpuInfo.Temperature : await GetGpuCurrentTemperatureAsync().ConfigureAwait(false);
        var gpuMaxTemperature = gpuInfo.MaxTemperature >= 0 ? gpuInfo.MaxTemperature : genericMaxTemperature;
        var gpuCurrentFanSpeed = await GetGpuCurrentFanSpeedAsync().ConfigureAwait(false);
        var gpuMaxFanSpeed = _gpuMaxFanSpeedCache ??= await GetGpuMaxFanSpeedAsync().ConfigureAwait(false);

        var cpu = new SensorData(cpuUtilization,
            genericMaxUtilization,
            cpuCoreClock,
            cpuMaxCoreClock,
            -1,
            -1,
            cpuCurrentTemperature,
            genericMaxTemperature,
            cpuCurrentFanSpeed,
            cpuMaxFanSpeed);
        var gpu = new SensorData(gpuInfo.Utilization,
            genericMaxUtilization,
            gpuInfo.CoreClock,
            gpuInfo.MaxCoreClock,
            gpuInfo.MemoryClock,
            gpuInfo.MaxMemoryClock,
            gpuCurrentTemperature,
            gpuMaxTemperature,
            gpuCurrentFanSpeed,
            gpuMaxFanSpeed);
        var result = new SensorsData(cpu, gpu);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Current data: {result} [type={GetType().Name}]");

        return result;
    }

    public async Task<(int cpuFanSpeed, int gpuFanSpeed)> GetFanSpeedsAsync()
    {
        var cpuFanSpeed = await GetCpuCurrentFanSpeedAsync().ConfigureAwait(false);
        var gpuFanSpeed = await GetGpuCurrentFanSpeedAsync().ConfigureAwait(false);
        return (cpuFanSpeed, gpuFanSpeed);
    }

    protected abstract Task<int> GetCpuCurrentTemperatureAsync();

    protected abstract Task<int> GetGpuCurrentTemperatureAsync();

    protected abstract Task<int> GetCpuCurrentFanSpeedAsync();

    protected abstract Task<int> GetGpuCurrentFanSpeedAsync();

    protected abstract Task<int> GetCpuMaxFanSpeedAsync();

    protected abstract Task<int> GetGpuMaxFanSpeedAsync();

    private int GetCpuUtilization(int maxUtilization)
    {
        var result = (int)_percentProcessorUtilityCounter.NextValue();
        if (result < 0)
            return -1;
        return Math.Min(result, maxUtilization);
    }

    private int GetCpuCoreClock()
    {
        var baseClock = _cpuBaseClockCache ??= GetCpuBaseClock();
        var clock = (int)(baseClock * (_percentProcessorPerformanceCounter.NextValue() / 100f));
        if (clock < 1)
            return -1;
        return clock;
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

    private async Task<GPUInfo> GetGPUInfoAsync()
    {
        if (gpuController.IsSupported())
            await gpuController.StartAsync().ConfigureAwait(false);

        if (await gpuController.GetLastKnownStateAsync().ConfigureAwait(false) is GPUState.PoweredOff or GPUState.Unknown)
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

            return new(utilization,
                currentCoreClock,
                maxCoreClock + maxCoreClockOffset,
                currentMemoryClock,
                maxMemoryClock + maxMemoryClockOffset,
                currentTemperature,
                maxTemperature);
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
