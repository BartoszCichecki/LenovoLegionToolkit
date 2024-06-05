using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace LenovoLegionToolkit.Lib.Controllers;

public class GPUOverclockController
{
    private readonly GPUOverclockSettings _settings;
    private readonly VantageDisabler _vantageDisabler;
    private readonly LegionZoneDisabler _legionZoneDisabler;
    private readonly NativeWindowsMessageListener _nativeWindowsMessageListener;

    public event EventHandler? Changed;

    public GPUOverclockController(GPUOverclockSettings settings,
        VantageDisabler vantageDisabler,
        LegionZoneDisabler legionZoneDisabler,
        NativeWindowsMessageListener nativeWindowsMessageListener)
    {
        _settings = settings;
        _vantageDisabler = vantageDisabler;
        _legionZoneDisabler = legionZoneDisabler;
        _nativeWindowsMessageListener = nativeWindowsMessageListener;
        _nativeWindowsMessageListener.Changed += NativeWindowsMessageListenerOnChanged;
    }

    public static int GetMaxCoreDeltaMhz() => 500;

    public static int GetMaxMemoryDeltaMhz()
    {
        try
        {
            NVAPI.Initialize();
            return GetMaxMemoryDeltaMhz(NVAPI.GetGPU());
        }
        finally
        {
            try { NVAPI.Unload(); } catch { /* Ignored */ }
        }
    }

    public async Task<bool> IsSupportedAsync()
    {
        bool isSupported;

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

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"NVAPI status: {isSupported}.");

        if (!isSupported)
            return isSupported;

        try
        {
            isSupported = await WMI.LenovoGameZoneData.IsSupportGpuOCAsync().ConfigureAwait(false) > 0;

            if (!isSupported)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Clearing settings...");

                _settings.Store.Enabled = false;
                _settings.Store.Info = GPUOverclockInfo.Zero;
                _settings.SynchronizeStore();
            }
        }
        catch
        {
            isSupported = false;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Supports GPU OC status: {isSupported}");

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

            Changed?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (await _legionZoneDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Can't correctly apply state when Legion Zone is running.");

            Changed?.Invoke(this, EventArgs.Empty);
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

            Changed?.Invoke(this, EventArgs.Empty);

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

                Changed?.Invoke(this, EventArgs.Empty);

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
            Changed?.Invoke(this, EventArgs.Empty);

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

    private async void NativeWindowsMessageListenerOnChanged(object? sender, NativeWindowsMessageListener.ChangedEventArgs e)
    {
        if (e.Message != NativeWindowsMessage.OnDisplayDeviceArrival)
            return;

        if (await IsSupportedAsync().ConfigureAwait(false))
            await ApplyStateAsync().ConfigureAwait(false);
    }

    private static int GetMaxMemoryDeltaMhz(PhysicalGPU? gpu) => gpu?.MemoryInformation.RAMMaker switch
    {
        GPUMemoryMaker.Samsung => 1500,
        _ => 750
    };

    private static void SetOverclockInfo(PhysicalGPU gpu, GPUOverclockInfo info)
    {
        var coreDelta = Math.Clamp(info.CoreDeltaMhz, 0, GetMaxCoreDeltaMhz());
        var memoryDelta = Math.Clamp(info.MemoryDeltaMhz, 0, GetMaxMemoryDeltaMhz(gpu));

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
        return new(core, memory);
    }
}
