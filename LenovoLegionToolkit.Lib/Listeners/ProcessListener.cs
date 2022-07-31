using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class ProcessListener : IListener<ProcessEventInfo>
    {
        private static readonly object _lock = new();

        private static readonly string[] _ignoredNames =
        {
            "backgroundTaskHost.exe",
            "CompPkgSrv.exe",
            "conhost.exe",
            "dllhost.exe",
            "msedgewebview2",
            "msedgewebview2.exe",
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

        public ProcessListener()
        {
            _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
            _instanceCreationListener.Changed += InstanceCreationListener_Changed;

            _instanceDeletionListener = new InstanceEventListener(ProcessEventInfoType.Stopped, "Win32_ProcessStopTrace");
            _instanceDeletionListener.Changed += InstanceDeletionListener_Changed;
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
                catch (ArgumentException) { }
                catch (InvalidOperationException) { }
                catch (Win32Exception) { }

                if (string.IsNullOrWhiteSpace(processPath))
                    return;

                if (_ignoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                var processInfo = new ProcessInfo(e.processName, processPath);
                _processCache[e.processID] = processInfo;

                Changed?.Invoke(this, new(e.type, processInfo));
            }
        }

        private void InstanceDeletionListener_Changed(object? sender, (ProcessEventInfoType type, int processID, string processName) e)
        {
            lock (_lock)
            {
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
