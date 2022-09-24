using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class GPUController
    {
        public enum Status
        {
            Unknown,
            NVIDIAGPUNotFound,
            MonitorsConnected,
            DeactivatePossible,
            Inactive,
        }

        public class RefreshedEventArgs : EventArgs
        {
            public bool IsActive { get; }
            public bool CanBeDeactivated { get; }
            public Status Status { get; }
            public string? PerformanceState { get; }
            public List<Process> Processes { get; }
            public int ProcessCount => Processes.Count;

            public RefreshedEventArgs(bool isActive, bool canBeDeactivated, Status status, string? performanceState, List<Process> processes)
            {
                IsActive = isActive;
                CanBeDeactivated = canBeDeactivated;
                Status = status;
                PerformanceState = performanceState;
                Processes = processes;
            }
        }

        private readonly AsyncLock _lock = new();

        private Task? _refreshTask;
        private CancellationTokenSource? _refreshCancellationTokenSource;

        private Status _status = Status.Unknown;
        private List<Process> _processes = new();
        private string? _gpuInstanceId;
        private string? _performanceState;

        public bool IsActive => _status is Status.MonitorsConnected or Status.DeactivatePossible;
        public bool CanBeDeactivated => _status == Status.DeactivatePossible;

        public event EventHandler? WillRefresh;
        public event EventHandler<RefreshedEventArgs>? Refreshed;

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
                catch { }
            }
        }

        public async Task<bool> CanBeDeactivatedAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                await RefreshLoopAsync(0, 0, CancellationToken.None);
                return CanBeDeactivated;
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

                        WillRefresh?.Invoke(this, EventArgs.Empty);
                        await RefreshStateAsync().ConfigureAwait(false);

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Refreshed");

                        Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDeactivated, _status, _performanceState, _processes));
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

            _status = Status.Unknown;
            _processes = new();
            _gpuInstanceId = null;
            _performanceState = null;

            var gpu = NVAPI.GetGPU();
            if (gpu is null)
            {
                _status = Status.NVIDIAGPUNotFound;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"GPU present [status={_status}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            try
            {
                var stateId = gpu.PerformanceStatesInfo.CurrentPerformanceState.StateId.ToString().GetUntilOrEmpty("_");
                _performanceState = "Powered On";
                if (!string.IsNullOrWhiteSpace(stateId))
                    _performanceState += $", {stateId}";

            }
            catch (Exception ex) when (ex.Message == "NVAPI_GPU_NOT_POWERED")
            {
                _performanceState = "Powered Off";
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
                _status = Status.Inactive;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"GPU inactive [status={_status}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            _processes = processNames;

            if (NVAPI.IsDisplayConnected(gpu))
            {
                _status = Status.MonitorsConnected;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Monitor connected [status={_status}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            var pnpDeviceId = NVAPI.GetGPUId(gpu);

            if (string.IsNullOrEmpty(pnpDeviceId))
                throw new InvalidOperationException("pnpDeviceId is null or empty");

            var gpuInstanceId = await GetDeviceInstanceIDAsync(pnpDeviceId).ConfigureAwait(false);

            _gpuInstanceId = gpuInstanceId;
            _status = Status.DeactivatePossible;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deactivate possible [status={_status}, processes.Count={_processes.Count}, gpuInstanceId={_gpuInstanceId}, pnpDeviceId={pnpDeviceId}]");
        }

        private static async Task<string?> GetDeviceInstanceIDAsync(string pnpDeviceId)
        {
            var results = await WMI.ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_PnpEntity WHERE DeviceID LIKE '{pnpDeviceId}%'",
                pdc => (string)pdc["DeviceID"].Value).ConfigureAwait(false);
            return results?.FirstOrDefault();
        }
    }
}
