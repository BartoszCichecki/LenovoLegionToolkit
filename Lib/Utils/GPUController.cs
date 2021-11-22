using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class GPUController
    {
        public enum Status
        {
            Unknown,
            Inactive,
            DeactivatePossible,
            SingleVideoCardFound,
            DiscreteNVGPUNotFound,
            MonitorsConnected,
        }

        public class RefreshedEventArgs : EventArgs
        {
            public bool IsActive { get; }
            public bool CanBeDisabled { get; }
            public Status Status { get; }
            public int ProcessCount { get; }
            public IEnumerable<string> ProcessNames { get; }

            public RefreshedEventArgs(bool isActive, bool canBeDisabled, Status status, int processCount, IEnumerable<string> processNames)
            {
                IsActive = isActive;
                CanBeDisabled = canBeDisabled;
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
        public IEnumerable<string> _processNames = null;
        private string _pnpDeviceId = null;

        private bool IsActive => _status != Status.Unknown && _status != Status.Inactive;
        private bool CanBeDisabled => _status == Status.DeactivatePossible;

        public event EventHandler<RefreshedEventArgs> Refreshed;

        public void Start(int interval = 2_000)
        {
            Stop(true);

            _refreshCancellationTokenSource = new CancellationTokenSource();
            var token = _refreshCancellationTokenSource.Token;

            _refreshTask = Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    lock (_lock)
                    {
                        Refresh();
                        Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDisabled, _status, _processCount, _processNames));
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
                if (!IsActive || !CanBeDisabled)
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

            var videoControllers = OS.GetVideoControllersInformation();
            if (videoControllers.Count() < 2)
            {
                _status = Status.SingleVideoCardFound;
                return;
            }

            var nvidiaVideoController = videoControllers
                .Where(vci => vci.IsNVidia())
                .Cast<VideoCardInformation?>()
                .FirstOrDefault();
            if (nvidiaVideoController == null)
            {
                _status = Status.DiscreteNVGPUNotFound;
                return;
            }

            var nvidiaInformation = OS.GetNVidiaInformation();
            if (nvidiaInformation.DisplayActive)
            {
                _status = Status.MonitorsConnected;
                _processCount = nvidiaInformation.ProcessCount;
                _processNames = nvidiaInformation.ProcessNames;
                return;
            }

            if (nvidiaInformation.ProcessCount < 1)
            {
                _status = Status.Inactive;
                return;
            }

            _status = Status.DeactivatePossible;
            _processCount = nvidiaInformation.ProcessCount;
            _processNames = nvidiaInformation.ProcessNames;
            _pnpDeviceId = nvidiaVideoController.Value.PnpDeviceId;
        }
    }
}
