using System;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly object _lock = new();

        private Task _refreshTask = null;
        private CancellationTokenSource _refreshCancellationTokenSource = null;

        private Status _status = Status.Unknown;
        private string[] _processNames = null;
        private string _pnpDeviceId = null;

        private bool IsActive => _status == Status.MonitorsConnected || _status == Status.DeactivatePossible;
        private bool CanBeDeactivated => _status == Status.DeactivatePossible;

        public event EventHandler WillRefresh;
        public event EventHandler<RefreshedEventArgs> Refreshed;

        public void Start(int delay = 2_500, int interval = 5_000)
        {
            Stop(true);

            _refreshCancellationTokenSource = new CancellationTokenSource();
            var token = _refreshCancellationTokenSource.Token;

            _refreshTask = Task.Run(async () =>
            {
                try
                {
                    NVAPI.Initialize();

                    await Task.Delay(delay, token);

                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        lock (_lock)
                        {
                            WillRefresh?.Invoke(this, EventArgs.Empty);
                            Refresh();
                            Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDeactivated, _status, _processNames));
                        }

                        await Task.Delay(interval, token);
                    }
                }
                finally
                {
                    NVAPI.Unload();
                }
            }, token);
        }

        public void Stop(bool waitForFinish = false)
        {
            _refreshCancellationTokenSource?.Cancel();

            if (waitForFinish)
                _refreshTask?.Wait();

            _refreshCancellationTokenSource = null;
            _refreshTask = null;
        }

        public void DeactivateGPU()
        {
            lock (_lock)
            {
                if (!IsActive || !CanBeDeactivated || string.IsNullOrEmpty(_pnpDeviceId))
                    return;

                CMD.Run("pnputil", $"/restart-device /deviceid \"{_pnpDeviceId}\"");
            }
        }

        private void Refresh()
        {
            _status = Status.Unknown;
            _processNames = Array.Empty<string>();
            _pnpDeviceId = null;

            if (!NVAPI.IsGPUPresent(out var gpu))
            {
                _status = Status.NVIDIAGPUNotFound;
                return;
            }

            var processNames = NVAPI.GetActiveApps(gpu);
            if (processNames.Length < 1)
            {
                _status = Status.Inactive;
                return;
            }

            _processNames = processNames;

            if (NVAPI.IsDisplayConnected(gpu))
            {
                _status = Status.MonitorsConnected;
                return;
            }

            _pnpDeviceId = NVAPI.GetGPUId(gpu);
            _status = Status.DeactivatePossible;
        }
    }
}
