using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace LenovoLegionToolkit.Lib.Controllers;

public class GPUOverclockController
{
    public const int MAX_CORE_DELTA_MHZ = 250;
    public const int MAX_MEMORY_DELTA_MHZ = 500;

    private readonly GPUOverclockSettings _settings;
    private readonly VantageDisabler _vantageDisabler;
    private readonly LegionZoneDisabler _legionZoneDisabler;

    public GPUOverclockController(GPUOverclockSettings settings, VantageDisabler vantageDisabler, LegionZoneDisabler legionZoneDisabler)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _vantageDisabler = vantageDisabler ?? throw new ArgumentNullException(nameof(vantageDisabler));
        _legionZoneDisabler = legionZoneDisabler ?? throw new ArgumentNullException(nameof(legionZoneDisabler));
    }

    public async Task<bool> IsSupportedAsync()
    {
        bool isSupported;

        try
        {
            isSupported = await WMI.CallAsync("ROOT\\WMI",
            $"SELECT * FROM LENOVO_GAMEZONE_DATA",
            "IsSupportGpuOC",
            new(),
            pdc => !pdc["Data"].Value.Equals(0)).ConfigureAwait(false);
        }
        catch
        {
            isSupported = false;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"IsSupportGpuOC={isSupported}");

        if (isSupported)
        {
            try
            {
                NVAPI.Initialize();
                isSupported = NVAPI.GetGPU() is not null;
            }
            catch
            {
                isSupported = false;
            }
            finally
            {
                try { NVAPI.Unload(); } catch { /* Ignored */ }
            }
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"NVAPI={isSupported}");

        if (!isSupported)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not supported, clearing settings.");

            _settings.Store.Enabled = false;
            _settings.Store.Info = GPUOverclockInfo.Zero;
            _settings.SynchronizeStore();
        }

        return isSupported;
    }

    public (bool, GPUOverclockInfo) GetState() => (_settings.Store.Enabled, _settings.Store.Info);

    public void SaveState(bool enabled, GPUOverclockInfo info)
    {
        _settings.Store.Enabled = enabled;
        _settings.Store.Info = info;
        _settings.SynchronizeStore();
    }

    public async Task ApplyStateAsync(bool force = false)
    {
        if (await _vantageDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Vantage is running.");
            return;
        }

        if (await _legionZoneDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");
            return;
        }

        var enabled = _settings.Store.Enabled;
        var info = _settings.Store.Info;

        if (force)
        {
            info = enabled ? info : GPUOverclockInfo.Zero;
            enabled = true;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Forcing... [enabled=true, info={info}]");
        }

        if (!enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Not enabled.");
            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Applying overclock: {info}.");

        try
        {
            NVAPI.Initialize();

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"dGPU not found.");

                return;
            }

            SetOverclockInfo(gpu, info);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Applied overclock: {info}, current: {GetOverclockInfo(gpu)}.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to apply overclock: {info}, clearing settings...", ex);

            _settings.Store.Enabled = false;
            _settings.Store.Info = GPUOverclockInfo.Zero;
            _settings.SynchronizeStore();
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }

    public async Task<bool> EnsureOverclockIsAppliedAsync()
    {
        var (enabled, _) = GetState();
        if (!enabled)
            return false;

        await ApplyStateAsync().ConfigureAwait(false);
        return true;
    }

    private static void SetOverclockInfo(PhysicalGPU gpu, GPUOverclockInfo info)
    {
        var coreDelta = Math.Clamp(info.CoreDeltaMhz, 0, MAX_CORE_DELTA_MHZ);
        var memoryDelta = Math.Clamp(info.MemoryDeltaMhz, 0, MAX_MEMORY_DELTA_MHZ);

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

    private static GPUOverclockInfo GetOverclockInfo(PhysicalGPU gpu)
    {
        var states = GPUApi.GetPerformanceStates20(gpu.Handle);
        var core = states.Clocks[PerformanceStateId.P0_3DPerformance][0].FrequencyDeltaInkHz.DeltaValue / 1000;
        var memory = states.Clocks[PerformanceStateId.P0_3DPerformance][1].FrequencyDeltaInkHz.DeltaValue / 1000;
        return new() { CoreDeltaMhz = core, MemoryDeltaMhz = memory };
    }
}
