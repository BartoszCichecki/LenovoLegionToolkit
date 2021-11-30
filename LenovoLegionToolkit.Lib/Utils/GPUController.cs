using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
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
            public int ProcessCount { get; }
            public IEnumerable<string> ProcessNames { get; }

            public RefreshedEventArgs(bool isActive, bool canBeDeactivated, Status status, int processCount, IEnumerable<string> processNames)
            {
                IsActive = isActive;
                CanBeDeactivated = canBeDeactivated;
                Status = status;
                ProcessCount = processCount;
                ProcessNames = processNames;
            }
        }

        private readonly object _lock = new();

        private Task _refreshTask = null;
        private CancellationTokenSource _refreshCancellationTokenSource = null;

        private Status _status = Status.Unknown;
        private int _processCount = -1;
        private IEnumerable<string> _processNames = null;
        private string _pnpDeviceId = null;

        private bool IsActive => _status == Status.MonitorsConnected || _status == Status.DeactivatePossible;
        private bool CanBeDeactivated => _status == Status.DeactivatePossible;

        public event EventHandler<RefreshedEventArgs> Refreshed;

        public void Start(int delay = 2_500, int interval = 5_000)
        {
            Stop(true);

            _refreshCancellationTokenSource = new CancellationTokenSource();
            var token = _refreshCancellationTokenSource.Token;

            _refreshTask = Task.Run(async () =>
            {
                await Task.Delay(delay, token);

                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    lock (_lock)
                    {
                        Refresh();
                        Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDeactivated, _status, _processCount, _processNames));
                    }

                    await Task.Delay(interval, token);
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
                if (!IsActive || !CanBeDeactivated)
                    return;

                OS.RestartDevice(_pnpDeviceId);
            }
        }

        private void Refresh()
        {
            _status = Status.Unknown;
            _processCount = -1;
            _processNames = null;
            _pnpDeviceId = null;

            if (!NVAPIWrapper.IsGPUPresent(out var gpu))
            {
                _status = Status.NVIDIAGPUNotFound;
                return;
            }

            if (!NVAPIWrapper.IsGPUActive(gpu))
            {
                _status = Status.Inactive;
                return;
            }

            var nvidiaInformation = OS.GetNVidiaInformation();

            _processCount = nvidiaInformation.ProcessCount;
            _processNames = nvidiaInformation.ProcessNames;

            if (NVAPIWrapper.IsDisplayConnected(gpu))
            {
                _status = Status.MonitorsConnected;
                return;
            }

            _pnpDeviceId = NVAPIWrapper.GetGPUId(gpu);
            _status = Status.DeactivatePossible;
        }
    }
}
