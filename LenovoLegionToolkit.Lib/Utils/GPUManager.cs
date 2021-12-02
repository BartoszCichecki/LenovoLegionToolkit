using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public event EventHandler WillRefresh;
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
                        WillRefresh?.Invoke(this, EventArgs.Empty);
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

                CMD.ExecuteProcess("pnputil", $"/restart-device /deviceid \"{_pnpDeviceId}\"");
            }
        }

        private void Refresh()
        {
            _status = Status.Unknown;
            _processCount = -1;
            _processNames = null;
            _pnpDeviceId = null;

            if (!NVAPI.IsGPUPresent(out var gpu))
            {
                _status = Status.NVIDIAGPUNotFound;
                return;
            }

            if (!NVAPI.IsGPUActive(gpu))
            {
                _status = Status.Inactive;
                return;
            }

            var (processCount, processNames) = GetProcessInformation();
            if (processCount == 0)
            {
                _status = Status.Inactive;
                return;
            }

            _processCount = processCount;
            _processNames = processNames;

            if (NVAPI.IsDisplayConnected(gpu))
            {
                _status = Status.MonitorsConnected;
                return;
            }

            _pnpDeviceId = NVAPI.GetGPUId(gpu);
            _status = Status.DeactivatePossible;
        }


        private static (int, IEnumerable<string>) GetProcessInformation()
        {
            var output = CMD.ExecuteProcessForOutput("nvidia-smi", "-q -x");

            var xdoc = XDocument.Parse(output);
            var gpu = xdoc.Element("nvidia_smi_log").Element("gpu");
            var processInfo = gpu.Element("processes").Elements("process_info");
            var processesCount = processInfo.Count();
            var processNames = processInfo.Select(e => e.Element("process_name").Value).Select(Path.GetFileName);

            return (processesCount, processNames);
        }
    }
}
