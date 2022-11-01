using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Listeners
{
    public class ProcessAutomationListener : IListener<ProcessEventInfo>
    {
        private static readonly object _lock = new();

        private static readonly string[] _ignoredNames =
        {
            "backgroundTaskHost.exe",
            "CompPkgSrv.exe",
            "conhost.exe",
            "dllhost.exe",
            "Lenovo Legion Toolkit.exe",
            "msedge.exe",
            "msedgewebview2.exe",
            "NvOAWrapperCache.exe",
            "SearchProtocolHost.exe",
            "svchost.exe",
            "WmiPrvSE.exe",
        };

        private static readonly string[] _ignoredPaths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        };

        public event EventHandler<ProcessEventInfo>? Changed;

        private readonly InstanceEventListener _instanceCreationListener;
        private readonly InstanceEventListener _instanceDeletionListener;

        private readonly Dictionary<int, ProcessInfo> _processCache = new();

        public ProcessAutomationListener()
        {
            _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
            _instanceCreationListener.Changed += InstanceCreationListener_Changed;

            _instanceDeletionListener = new InstanceEventListener(ProcessEventInfoType.Stopped, "Win32_ProcessStopTrace");
            _instanceDeletionListener.Changed += InstanceDeletionListener_Changed;
        }

        public async Task StartAsync()
        {
            await _instanceCreationListener.StartAsync().ConfigureAwait(false);
            await _instanceDeletionListener.StartAsync().ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await _instanceCreationListener.StopAsync().ConfigureAwait(false);
            await _instanceDeletionListener.StopAsync().ConfigureAwait(false);

            _processCache.Clear();
        }

        private void InstanceCreationListener_Changed(object? sender, (ProcessEventInfoType type, int processID, string processName) e)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(e.processName) || _ignoredNames.Contains(e.processName, StringComparer.InvariantCultureIgnoreCase))
                    return;

                if (e.processID < 0)
                    return;

                var processPath = "";
                try
                {
                    processPath = Process.GetProcessById(e.processID).MainModule?.FileName;
                }
                catch (Exception)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Can't get process {e.processName} details, fallback to ID only.");
                }

                if (processPath is not null && _ignoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                var processInfo = new ProcessInfo(Path.GetFileNameWithoutExtension(e.processName), processPath);
                _processCache[e.processID] = processInfo;

                Changed?.Invoke(this, new(e.type, processInfo));
            }
        }

        private void InstanceDeletionListener_Changed(object? sender, (ProcessEventInfoType type, int processID, string processName) e)
        {
            lock (_lock)
            {
                CleanUpCacheIfNecessary();

                if (string.IsNullOrWhiteSpace(e.processName) || _ignoredNames.Contains(e.processName, StringComparer.InvariantCultureIgnoreCase))
                    return;

                if (e.processID < 0)
                    return;

                if (!_processCache.TryGetValue(e.processID, out var processInfo))
                    return;

                _processCache.Remove(e.processID);

                Changed?.Invoke(this, new(e.type, processInfo));
            }
        }

        private void CleanUpCacheIfNecessary()
        {
            if (_processCache.Count < 100)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Cleaning up process cache. Current size: {_processCache.Count}.");

            foreach (var (processId, _) in _processCache)
            {
                try
                {
                    _ = Process.GetProcessById(processId);
                }
                catch (ArgumentException)
                {
                    _processCache.Remove(processId);
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Cleaned up process cache. Current size: {_processCache.Count}.");
        }

        private class InstanceEventListener : AbstractWMIListener<(ProcessEventInfoType, int, string)>
        {
            private readonly ProcessEventInfoType _type;

            public InstanceEventListener(ProcessEventInfoType type, string eventName)
                : base("ROOT\\CIMV2", query: $"SELECT * FROM {eventName}")
            {
                _type = type;
            }

            protected override (ProcessEventInfoType, int, string) GetValue(PropertyDataCollection properties)
            {
                var processName = properties["ProcessName"].Value?.ToString() ?? string.Empty;
                if (!int.TryParse(properties["ProcessID"].Value?.ToString(), out int processID))
                    processID = -1;

                return (_type, processID, processName);
            }

            protected override Task OnChangedAsync((ProcessEventInfoType, int, string) value) => Task.CompletedTask;
        }
    }
}
