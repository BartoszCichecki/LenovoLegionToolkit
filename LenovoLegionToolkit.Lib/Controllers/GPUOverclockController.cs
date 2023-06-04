using System;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace LenovoLegionToolkit.Lib.Controllers;

public class GPUOverclockController
{
    public const int MAX_CORE_DELTA_MHZ = 250;
    public const int MAX_MEMORY_DELTA_MHZ = 250;

    private readonly GPUOverclockSettings _settings;

    public GPUOverclockController(GPUOverclockSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public bool IsSupported()
    {
        try
        {
            NVAPI.Initialize();
            return NVAPI.GetGPU() is not null;
        }
        catch
        {
            return false;
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }

    public GPUOverclockInfo? GetState(PowerModeState powerMode)
    {
        if (powerMode == PowerModeState.Performance)
            return _settings.Store.PerformanceModeGpuOverclockInfo;

        return null;
    }

    public void SaveState(GPUOverclockInfo? info, PowerModeState powerMode)
    {
        if (powerMode == PowerModeState.Performance)
            _settings.Store.PerformanceModeGpuOverclockInfo = info;

        _settings.SynchronizeStore();
    }

    public void ApplyState(PowerModeState powerMode)
    {
        GPUOverclockInfo? info = null;

        if (powerMode == PowerModeState.Performance)
        {
            info = _settings.Store.PerformanceModeGpuOverclockInfo;

            if (!info.HasValue && !GPUOverclockInfo.Zero.Equals(GetCurrentState()))
                info = GPUOverclockInfo.Zero;
        }
        else if (_settings.Store.PerformanceModeGpuOverclockInfo.HasValue)
        {
            info = GPUOverclockInfo.Zero;
        }

        if (!info.HasValue)
            return;

        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
                return;

            var coreDelta = Math.Clamp(info.Value.CoreDeltaMhz, -MAX_CORE_DELTA_MHZ, MAX_CORE_DELTA_MHZ);
            var memoryDelta = Math.Clamp(info.Value.MemoryDeltaMhz, -MAX_MEMORY_DELTA_MHZ, MAX_MEMORY_DELTA_MHZ);

            var clockEntries = new[]
            {
                new PerformanceStates20ClockEntryV1(PublicClockDomain.Graphics, new PerformanceStates20ParameterDelta(coreDelta * 1000)),
                new PerformanceStates20ClockEntryV1(PublicClockDomain.Memory, new PerformanceStates20ParameterDelta(memoryDelta * 1000))
            };
            var voltageEntries = Array.Empty<PerformanceStates20BaseVoltageEntryV1>();
            var performanceStateInfo = new[] { new PerformanceStates20InfoV1.PerformanceState20(PerformanceStateId.P0_3DPerformance, clockEntries, voltageEntries) };

            var overclock = new PerformanceStates20InfoV1(performanceStateInfo, 2, 0);
            GPUApi.SetPerformanceStates20(gpu.Handle, overclock);
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }
    private GPUOverclockInfo? GetCurrentState()
    {
        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
                return null;

            var states = GPUApi.GetPerformanceStates20(gpu.Handle);
            var core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
            var memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;
            return new() { CoreDeltaMhz = core, MemoryDeltaMhz = memory };
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }
}
