using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class NVidiaMonitor
    {
        public enum Status
        {
            Unknown,
            Inactive,
            UnnecessarilyActive,
            SingleVideoCardFound,
            DiscreteGPUNotFound,
            MonitorsConnected,
        }

        public class RefreshedEventArgs : EventArgs
        {
            public bool IsActive { get; }
            public bool CanBeDisabled { get; }
            public Status Status { get; }
            public int ProcessCount { get; }

            public RefreshedEventArgs(bool isActive, bool canBeDisabled, Status status, int processCount)
            {
                IsActive = isActive;
                CanBeDisabled = canBeDisabled;
                Status = status;
                ProcessCount = processCount;
            }
        }

        private readonly object _lock = new();

        private Task _refreshTask = null;
        private CancellationTokenSource _refreshCancellationTokenSource = null;

        private Status _status = Status.Unknown;
        private int _processCount = -1;
        private string _pnpDeviceId = null;

        private bool IsActive => _status != Status.Unknown && _status != Status.Inactive;
        private bool CanBeDisabled => _status == Status.UnnecessarilyActive;

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
                        Refreshed?.Invoke(this, new RefreshedEventArgs(IsActive, CanBeDisabled, _status, _processCount));
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

        public void DisableGPU()
        {
            lock (_lock)
            {
                if (!IsActive || !CanBeDisabled)
                    return;


            }
        }

        private void Refresh()
        {
            _status = Status.Unknown;
            _processCount = -1;
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
                _status = Status.DiscreteGPUNotFound;
                return;
            }

            var nvidiaInformation = OS.GetNVidiaInformation();
            if (nvidiaInformation.DisplayActive)
            {
                _status = Status.MonitorsConnected;
                _processCount = nvidiaInformation.ProcessCount;
                return;
            }

            if (nvidiaInformation.ProcessCount < 1)
            {
                _status = Status.Inactive;
                return;
            }

            _status = Status.UnnecessarilyActive;
            _processCount = nvidiaInformation.ProcessCount;
            _pnpDeviceId = nvidiaVideoController.Value.PnpDeviceId;
        }
    }
}
