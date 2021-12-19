using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class GPUManager
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
            public string[] ProcessNames { get; }
            public int ProcessCount => ProcessNames.Length;

            public RefreshedEventArgs(bool isActive, bool canBeDeactivated, Status status, string[] processNames)
            {
                IsActive = isActive;
                CanBeDeactivated = canBeDeactivated;
                Status = status;
                ProcessNames = processNames;
            }
        }

        private readonly AsyncLock _lock = new();

        private Task _refreshTask = null;
        private CancellationTokenSource _refreshCancellationTokenSource = null;

        private Status _status = Status.Unknown;
        private string[] _processNames = Array.Empty<string>();
        private string _gpuInstanceId = null;

        private bool IsActive => _status == Status.MonitorsConnected || _status == Status.DeactivatePossible;
        private bool CanBeDeactivated => _status == Status.DeactivatePossible;

        public event EventHandler WillRefresh;
        public event EventHandler<RefreshedEventArgs> Refreshed;

        public async Task StartAsync(int delay = 2_500, int interval = 5_000)
        {
            await StopAsync(true);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [delay={delay}, interval={interval}]");

            _refreshCancellationTokenSource = new CancellationTokenSource();
            var token = _refreshCancellationTokenSource.Token;

            _refreshTask = Task.Run(async () =>
            {
                try
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Initializing NVAPI...");

                    NVAPI.Initialize();

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Initialized NVAPI");

                    await Task.Delay(delay, token);

                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        using (await _lock.LockAsync())
                        {

                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Will refresh...");

                            WillRefresh?.Invoke(this, EventArgs.Empty);
                            await RefreshAsync();

                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Refreshed");

                            Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDeactivated, _status, _processNames));
                        }

                        await Task.Delay(interval, token);
                    }
                }
                catch (Exception ex) when (ex is not TaskCanceledException)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Exception: {ex}");

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
            }, token);
        }

        public async Task StopAsync(bool waitForFinish = false)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping... [refreshTask.isNull={_refreshTask == null}, _refreshCancellationTokenSource.IsCancellationRequested={_refreshCancellationTokenSource?.IsCancellationRequested}]");

            _refreshCancellationTokenSource?.Cancel();

            if (waitForFinish)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Waiting to finish...");

                if (_refreshTask != null)
                    await _refreshTask;

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
            using (await _lock.LockAsync())

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Deactivating... [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");

            if (!IsActive || !CanBeDeactivated || string.IsNullOrEmpty(_gpuInstanceId))
                return;

            await CMD.RunAsync("pnputil", $"/restart-device \"{_gpuInstanceId}\"");

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deactivated [isActive={IsActive}, canBeDeactivated={CanBeDeactivated}, gpuInstanceId={_gpuInstanceId}]");
        }

        private async Task RefreshAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Refresh in progress...");

            _status = Status.Unknown;
            _processNames = Array.Empty<string>();
            _gpuInstanceId = null;

            if (!NVAPI.IsGPUPresent(out var gpu))
            {
                _status = Status.NVIDIAGPUNotFound;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"GPU present [status={_status}, processNames.Length={_processNames.Length}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            var processNames = NVAPI.GetActiveApps(gpu);
            if (processNames.Length < 1)
            {
                _status = Status.Inactive;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"GPU inactive [status={_status}, processNames.Length={_processNames.Length}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            _processNames = processNames;

            if (NVAPI.IsDisplayConnected(gpu))
            {
                _status = Status.MonitorsConnected;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Monitor connected [status={_status}, processNames.Length={_processNames.Length}, gpuInstanceId={_gpuInstanceId}]");

                return;
            }

            var pnpDeviceId = NVAPI.GetGPUId(gpu);
            var gpuInstanceId = await GetDeviceInstanceIDAsync(pnpDeviceId);

            _gpuInstanceId = gpuInstanceId;
            _status = Status.DeactivatePossible;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deactivate possible [status={_status}, processNames.Length={_processNames.Length}, gpuInstanceId={_gpuInstanceId}, pnpDeviceId={pnpDeviceId}]");
        }

        private static async Task<string> GetDeviceInstanceIDAsync(string pnpDeviceId)
        {
            return (await WMI.ReadAsync("root\\CIMV2",
                $"SELECT * FROM Win32_PnpEntity WHERE DeviceID LIKE '{pnpDeviceId}%'",
                pdc => (string)pdc["DeviceID"].Value)).FirstOrDefault();
        }
    }
}
