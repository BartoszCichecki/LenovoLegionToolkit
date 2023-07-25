using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class ProcessAutoListener : AbstractAutoListener<ProcessEventInfo>
{
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

    private readonly InstanceStartedEventAutoAutoListener _instanceStartedEventAutoAutoListener;
    private readonly InstanceStoppedEventAutoAutoListener _instanceStoppedEventAutoAutoListener;

    private readonly Dictionary<int, ProcessInfo> _processCache = new();

    public ProcessAutoListener(InstanceStartedEventAutoAutoListener instanceStartedEventAutoAutoListener, InstanceStoppedEventAutoAutoListener instanceStoppedEventAutoAutoListener)
    {
        _instanceStartedEventAutoAutoListener = instanceStartedEventAutoAutoListener ?? throw new ArgumentNullException(nameof(instanceStartedEventAutoAutoListener));
        _instanceStoppedEventAutoAutoListener = instanceStoppedEventAutoAutoListener ?? throw new ArgumentNullException(nameof(instanceStoppedEventAutoAutoListener));

    }

    protected override Task StartAsync()
    {
        _instanceStartedEventAutoAutoListener.Changed += InstanceStartedEventAutoAutoListener_Changed;
        _instanceStoppedEventAutoAutoListener.Changed += InstanceStoppedEventAutoAutoListener_Changed;

        return Task.CompletedTask;
    }

    protected override Task StopAsync()
    {
        _instanceStartedEventAutoAutoListener.Changed -= InstanceStartedEventAutoAutoListener_Changed;
        _instanceStoppedEventAutoAutoListener.Changed -= InstanceStoppedEventAutoAutoListener_Changed;

        lock (Lock)
            _processCache.Clear();

        return Task.CompletedTask;
    }

    private void InstanceStartedEventAutoAutoListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
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

    private void InstanceStoppedEventAutoAutoListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
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
