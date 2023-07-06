using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Resources;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers;

public class GPUController
{
    public enum GPUState
    {
        Unknown,
        NvidiaGpuNotFound,
        MonitorsConnected,
        DeactivatePossible,
        Inactive,
        PoweredOff
    }

    public readonly struct GPUStatus
    {
        public GPUState State { get; }
        public string? PerformanceState { get; }
        public List<Process> Processes { get; }
        public int ProcessCount => Processes.Count;
        public bool IsActive => State is GPUState.MonitorsConnected or GPUState.DeactivatePossible;
        public bool IsPoweredOff => State is GPUState.PoweredOff;
        public bool CanBeDeactivated => State == GPUState.DeactivatePossible;

        public GPUStatus(GPUState state, string? performanceState, List<Process> processes)
        {
            State = state;
            PerformanceState = performanceState;
            Processes = processes;
        }
    }

    private readonly AsyncLock _lock = new();

    private Task? _refreshTask;
    private CancellationTokenSource? _refreshCancellationTokenSource;

    private GPUState _state = GPUState.Unknown;
    private List<Process> _processes = new();
    private string? _gpuInstanceId;
    private string? _performanceState;

    private bool IsActive => _state is GPUState.MonitorsConnected or GPUState.DeactivatePossible;
    private bool CanBeDeactivated => _state is GPUState.DeactivatePossible;

    public GPUState LastKnownState => _state;
    public event EventHandler<GPUStatus>? Refreshed;

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
            try
            {
                NVAPI.Unload();
            }
            catch { /* Ignored. */ }
        }
    }

    public async Task<GPUStatus> RefreshNowAsync()
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            await RefreshLoopAsync(0, 0, CancellationToken.None);
            return new GPUStatus(_state, _performanceState, _processes);
        }
    }

    public async Task StartAsync(int delay = 1_000, int interval = 5_000)
    {
        await StopAsync(true).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting... [delay={delay}, interval={interval}]");

        _refreshCancellationTokenSource = new CancellationTokenSource();
        var token = _refreshCancellationTokenSource.Token;
        _refreshTask = Task.Run(() => RefreshLoopAsync(delay, interval, token), token);
    }

    public async Task StopAsync(bool waitForFinish = false)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping... [refreshTask.isNull={_refreshTask is null}, _refreshCancellationTokenSource.IsCancellationRequested={_refreshCancellationTokenSource?.IsCancellationRequested}]");

        _refreshCancellationTokenSource?.Cancel();

        if (waitForFinish)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Waiting to finish...");

            if (_refreshTask is not null)
            {
                try
                {
                    await _refreshTask.ConfigureAwait(false);
                }
                catch (TaskCanceledException) { }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Finished");
        }

        _refreshCancellationTokenSource = null;
        _refreshTask = null;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped");
    }

    public async Task DeactivateGPUAsync()
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deactivating... [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");

            if (!IsActive || !CanBeDeactivated || string.IsNullOrEmpty(_gpuInstanceId))
                return;

            await CMD.RunAsync("pnputil", $"/restart-device \"{_gpuInstanceId}\"").ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deactivated [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");
        }
    }

    public async Task KillGPUProcessesAsync()
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Killing GPU processes... [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");

            if (!IsActive || !CanBeDeactivated || string.IsNullOrEmpty(_gpuInstanceId))
                return;

            foreach (var process in _processes)
            {
                try
                {
                    process.Kill(true);
                    await process.WaitForExitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Couldn't kill process. [pid={process.Id}, name={process.ProcessName}]", ex);
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Killed GPU processes. [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");
        }
    }

    private async Task RefreshLoopAsync(int delay, int interval, CancellationToken token)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Initializing NVAPI...");

            NVAPI.Initialize();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Initialized NVAPI");

            await Task.Delay(delay, token).ConfigureAwait(false);

            while (true)
            {
                token.ThrowIfCancellationRequested();

                using (await _lock.LockAsync(token).ConfigureAwait(false))
                {

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Will refresh...");

                    await RefreshStateAsync().ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Refreshed");

                    Refreshed?.Invoke(this, new GPUStatus(_state, _performanceState, _processes));
                }

                if (interval > 0)
                    await Task.Delay(interval, token).ConfigureAwait(false);
                else
                    break;
            }
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Exception occurred", ex);

            throw;
        }
        finally
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unloading NVAPI...");

            NVAPI.Unload();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unloaded NVAPI");
        }
    }

    private async Task RefreshStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Refresh in progress...");

        _state = GPUState.Unknown;
        _processes = new();
        _gpuInstanceId = null;
        _performanceState = null;

        var gpu = NVAPI.GetGPU();
        if (gpu is null)
        {
            _state = GPUState.NvidiaGpuNotFound;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"GPU present [state={_state}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

            return;
        }

        try
        {
            var stateId = gpu.PerformanceStatesInfo.CurrentPerformanceState.StateId.ToString().GetUntilOrEmpty("_");
            _performanceState = Resource.GPUController_PoweredOn;
            if (!string.IsNullOrWhiteSpace(stateId))
                _performanceState += $", {stateId}";
        }
        catch (Exception ex) when (ex.Message == "NVAPI_GPU_NOT_POWERED")
        {
            _state = GPUState.PoweredOff;
            _performanceState = Resource.GPUController_PoweredOff;
            return;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"GPU status exception.", ex);

            _performanceState = "Unknown";
        }

        var processNames = NVAPIExtensions.GetActiveProcesses(gpu);
        if (processNames.Count < 1)
        {
            _state = GPUState.Inactive;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"GPU inactive [state={_state}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

            return;
        }

        _processes = processNames;

        if (NVAPI.IsDisplayConnected(gpu))
        {
            _state = GPUState.MonitorsConnected;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Monitor connected [state={_state}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

            return;
        }

        var pnpDeviceId = NVAPI.GetGPUId(gpu);

        if (string.IsNullOrEmpty(pnpDeviceId))
            throw new InvalidOperationException("pnpDeviceId is null or empty");

        var gpuInstanceId = await GetDeviceInstanceIDAsync(pnpDeviceId).ConfigureAwait(false);

        _state = GPUState.DeactivatePossible;
        _gpuInstanceId = gpuInstanceId;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Deactivate possible [state={_state}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}, pnpDeviceId={pnpDeviceId}]");
    }

    private static async Task<string?> GetDeviceInstanceIDAsync(string pnpDeviceId)
    {
        var results = await WMI.ReadAsync("root\\CIMV2",
            $"SELECT * FROM Win32_PnpEntity WHERE DeviceID LIKE '{pnpDeviceId}%'",
            pdc => (string)pdc["DeviceID"].Value).ConfigureAwait(false);
        return results.FirstOrDefault();
    }
}
