using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
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

    private readonly PerformanceCounter _percentProcessorPerformanceCounter = new("Processor Information", "% Processor Performance", "_Total");
    private readonly PerformanceCounter _percentProcessorUtilityCounter = new("Processor Information", "% Processor Utility", "_Total");

    private readonly GPUController _gpuController;

    private int? _cpuBaseClockCache;
    private int? _cpuMaxCoreClockCache;
    private int? _cpuMaxFanSpeedCache;
    private int? _gpuMaxFanSpeedCache;

    protected readonly SensorSettings Settings;

    protected AbstractSensorsController(SensorSettings settings, GPUController gpuController)
    {
        Settings = settings;
        _gpuController = gpuController ?? throw new ArgumentNullException(nameof(gpuController));
    }

    public virtual async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {Settings.CPUSensorID} AND Fan_Id = {Settings.CPUFanID}").ConfigureAwait(false);
            result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = {Settings.GPUSensorID} AND Fan_Id = {Settings.GPUFanID}").ConfigureAwait(false);

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

    public async Task<SensorsData> GetDataAsync()
    {
        var cpuUtilization = GetCpuUtilization();
        var cpuCoreClock = GetCpuCoreClock();
        var cpuMaxCoreClock = _cpuMaxCoreClockCache ??= await GetCpuMaxCoreClockAsync().ConfigureAwait(false);
        var cpuCurrentTemperature = await GetCurrentTemperatureAsync(Settings.CPUSensorID).ConfigureAwait(false);
        var cpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.CPUFanID).ConfigureAwait(false);
        var cpuMaxFanSpeed = _cpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.CPUSensorID, Settings.CPUFanID).ConfigureAwait(false);

        var gpuInfo = GetGPUInfo();
        var gpuCurrentFanSpeed = await GetCurrentFanSpeedAsync(Settings.GPUFanID).ConfigureAwait(false);
        var gpuMaxFanSpeed = _gpuMaxFanSpeedCache ??= await GetMaxFanSpeedAsync(Settings.GPUSensorID, Settings.GPUFanID).ConfigureAwait(false);

        return new()
        {
            CPU = new()
            {
                Utilization = cpuUtilization,
                MaxUtilization = 100,
                CoreClock = cpuCoreClock,
                MaxCoreClock = cpuMaxCoreClock,
                Temperature = cpuCurrentTemperature,
                MaxTemperature = 100,
                FanSpeed = cpuCurrentFanSpeed,
                MaxFanSpeed = cpuMaxFanSpeed,
            },
            GPU = new()
            {
                Utilization = gpuInfo.Utilization,
                MaxUtilization = 100,
                CoreClock = gpuInfo.CoreClock,
                MaxCoreClock = gpuInfo.MaxCoreClock,
                MemoryClock = gpuInfo.MemoryClock,
                MaxMemoryClock = gpuInfo.MaxMemoryClock,
                Temperature = gpuInfo.Temperature,
                MaxTemperature = gpuInfo.MaxTemperature,
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

    private int GetCpuUtilization()
    {
        var result = _percentProcessorUtilityCounter.NextValue();
        return result == 0.0 ? -1 : (int)result;
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
