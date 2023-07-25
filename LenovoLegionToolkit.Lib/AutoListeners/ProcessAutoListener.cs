using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class ProcessAutoListener : AbstractAutoListener<ProcessEventInfo>
{
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
            // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            var processName = properties["ProcessName"].Value?.ToString() ?? string.Empty;
            if (!int.TryParse(properties["ProcessID"].Value?.ToString(), out var processId))
                processId = -1;

            return (_type, processId, Path.GetFileNameWithoutExtension(processName));
            // ReSharper enable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        }

        protected override Task OnChangedAsync((ProcessEventInfoType, int, string) value) => Task.CompletedTask;
    }

    private static readonly object Lock = new();

    // ReSharper disable StringLiteralTypo

    private static readonly string[] IgnoredNames =
    {
        "backgroundTaskHost",
        "CompPkgSrv",
        "conhost",
        "dllhost",
        "Lenovo Legion Toolkit",
        "msedge",
        "msedgewebview2",
        "NvOAWrapperCache",
        "SearchProtocolHost",
        "svchost",
        "WmiPrvSE"
    };

    // ReSharper restore StringLiteralTypo

    private static readonly string[] IgnoredPaths =
    {
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
    };

    private readonly InstanceEventListener _instanceCreationListener;
    private readonly InstanceEventListener _instanceDeletionListener;

    private readonly Dictionary<int, ProcessInfo> _processCache = new();

    public ProcessAutoListener()
    {
        _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
        _instanceCreationListener.Changed += InstanceCreationListener_Changed;

        _instanceDeletionListener = new InstanceEventListener(ProcessEventInfoType.Stopped, "Win32_ProcessStopTrace");
        _instanceDeletionListener.Changed += InstanceDeletionListener_Changed;
    }

    protected override async Task StartAsync()
    {
        await _instanceCreationListener.StartAsync().ConfigureAwait(false);
        await _instanceDeletionListener.StartAsync().ConfigureAwait(false);
    }

    protected override async Task StopAsync()
    {
        await _instanceCreationListener.StopAsync().ConfigureAwait(false);
        await _instanceDeletionListener.StopAsync().ConfigureAwait(false);

        lock (Lock)
            _processCache.Clear();
    }

    private void InstanceCreationListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
    {
        lock (Lock)
        {
            if (e.processId < 0)
                return;

            if (string.IsNullOrWhiteSpace(e.processName))
                return;

            if (IgnoredNames.Contains(e.processName, StringComparer.InvariantCultureIgnoreCase))
                return;

            string? processPath = null;
            try
            {
                processPath = Process.GetProcessById(e.processId).GetFileName();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {e.processName} details.", ex);
            }

            if (!string.IsNullOrEmpty(processPath) && IgnoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var processInfo = new ProcessInfo(e.processName, processPath);
            _processCache[e.processId] = processInfo;

            RaiseChanged(new(e.type, processInfo));
        }
    }

    private void InstanceDeletionListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
    {
        lock (Lock)
        {
            CleanUpCacheIfNecessary();

            if (e.processId < 0)
                return;

            if (string.IsNullOrWhiteSpace(e.processName))
                return;

            if (IgnoredNames.Contains(e.processName, StringComparer.InvariantCultureIgnoreCase))
                return;

            if (!_processCache.TryGetValue(e.processId, out var processInfo))
                return;

            _processCache.Remove(e.processId);

            RaiseChanged(new(e.type, processInfo));
        }
    }

    private void CleanUpCacheIfNecessary()
    {
        if (_processCache.Count < 250)
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
}
