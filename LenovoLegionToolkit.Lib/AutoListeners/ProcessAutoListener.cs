using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class ProcessAutoListener(
    InstanceStartedEventAutoAutoListener instanceStartedEventAutoAutoListener,
    InstanceStoppedEventAutoAutoListener instanceStoppedEventAutoAutoListener)
    : AbstractAutoListener<ProcessAutoListener.ChangedEventArgs>
{
    public class ChangedEventArgs(ProcessEventInfoType type, ProcessInfo processInfo) : EventArgs
    {
        public ProcessEventInfoType Type { get; } = type;

        public ProcessInfo ProcessInfo { get; } = processInfo;
    }

    private static readonly object Lock = new();

    // ReSharper disable StringLiteralTypo

    private static readonly string[] IgnoredNames =
    [
        "backgroundTaskHost",
        "cmd",
        "CompPkgSrv",
        "conhost",
        "dllhost",
        "Lenovo Legion Toolkit",
        "msedge",
        "msedgewebview2",
        "NvOAWrapperCache",
        "SearchProtocolHost",
        "svchost",
        "taskhostw",
        "WmiApSrv",
        "WmiPrvSE"
    ];

    // ReSharper restore StringLiteralTypo

    private static readonly string[] IgnoredPaths =
    [
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
    ];

    private readonly Dictionary<int, ProcessInfo> _processCache = [];

    protected override async Task StartAsync()
    {
        await instanceStartedEventAutoAutoListener.SubscribeChangedAsync(InstanceStartedEventAutoListener_Changed).ConfigureAwait(false);
        await instanceStoppedEventAutoAutoListener.SubscribeChangedAsync(InstanceStoppedEventAutoListener_Changed).ConfigureAwait(false);
    }

    protected override async Task StopAsync()
    {
        await instanceStartedEventAutoAutoListener.UnsubscribeChangedAsync(InstanceStartedEventAutoListener_Changed).ConfigureAwait(false);
        await instanceStoppedEventAutoAutoListener.UnsubscribeChangedAsync(InstanceStoppedEventAutoListener_Changed).ConfigureAwait(false);

        lock (Lock)
            _processCache.Clear();
    }

    private void InstanceStartedEventAutoListener_Changed(object? sender, InstanceStartedEventAutoAutoListener.ChangedEventArgs e)
    {
        lock (Lock)
        {
            if (e.ProcessId < 0)
                return;

            if (string.IsNullOrWhiteSpace(e.ProcessName))
                return;

            if (IgnoredNames.Contains(e.ProcessName, StringComparer.InvariantCultureIgnoreCase))
                return;

            string? processPath = null;
            try
            {
                processPath = Process.GetProcessById(e.ProcessId).GetFileName();
            }
            catch (ArgumentException)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Process {e.ProcessName} isn't running, ignoring... [processId={e.ProcessId}]");
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {e.ProcessName} details. [processId={e.ProcessId}]", ex);
            }

            if (!string.IsNullOrEmpty(processPath) && IgnoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var processInfo = new ProcessInfo(e.ProcessName, processPath);
            _processCache[e.ProcessId] = processInfo;

            CleanUpCacheIfNecessary();

            RaiseChanged(new ChangedEventArgs(ProcessEventInfoType.Started, processInfo));
        }
    }

    private void InstanceStoppedEventAutoListener_Changed(object? sender, InstanceStoppedEventAutoAutoListener.ChangedEventArgs e)
    {
        lock (Lock)
        {
            CleanUpCacheIfNecessary();

            if (e.ProcessId < 0)
                return;

            if (string.IsNullOrWhiteSpace(e.ProcessName))
                return;

            if (IgnoredNames.Contains(e.ProcessName, StringComparer.InvariantCultureIgnoreCase))
                return;

            if (!_processCache.Remove(e.ProcessId, out var processInfo))
                return;

            RaiseChanged(new ChangedEventArgs(ProcessEventInfoType.Stopped, processInfo));
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
