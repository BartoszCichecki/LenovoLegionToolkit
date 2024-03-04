﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class ProcessAutoListener : AbstractAutoListener<ProcessAutoListener.ChangedEventArgs>
{
    public class ChangedEventArgs : EventArgs
    {
        public ProcessEventInfoType Type { get; init; }

        public ProcessInfo ProcessInfo { get; init; }
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

    private readonly InstanceStartedEventAutoAutoListener _instanceStartedEventAutoAutoListener;
    private readonly InstanceStoppedEventAutoAutoListener _instanceStoppedEventAutoAutoListener;

    private readonly Dictionary<int, ProcessInfo> _processCache = new();

    public ProcessAutoListener(InstanceStartedEventAutoAutoListener instanceStartedEventAutoAutoListener, InstanceStoppedEventAutoAutoListener instanceStoppedEventAutoAutoListener)
    {
        _instanceStartedEventAutoAutoListener = instanceStartedEventAutoAutoListener ?? throw new ArgumentNullException(nameof(instanceStartedEventAutoAutoListener));
        _instanceStoppedEventAutoAutoListener = instanceStoppedEventAutoAutoListener ?? throw new ArgumentNullException(nameof(instanceStoppedEventAutoAutoListener));

    }

    protected override async Task StartAsync()
    {
        await _instanceStartedEventAutoAutoListener.SubscribeChangedAsync(InstanceStartedEventAutoListener_Changed).ConfigureAwait(false);
        await _instanceStoppedEventAutoAutoListener.SubscribeChangedAsync(InstanceStoppedEventAutoListener_Changed).ConfigureAwait(false);
    }

    protected override async Task StopAsync()
    {
        await _instanceStartedEventAutoAutoListener.UnsubscribeChangedAsync(InstanceStartedEventAutoListener_Changed).ConfigureAwait(false);
        await _instanceStoppedEventAutoAutoListener.UnsubscribeChangedAsync(InstanceStoppedEventAutoListener_Changed).ConfigureAwait(false);

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
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {e.ProcessName} details.", ex);
            }

            if (!string.IsNullOrEmpty(processPath) && IgnoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var processInfo = new ProcessInfo(e.ProcessName, processPath);
            _processCache[e.ProcessId] = processInfo;

            RaiseChanged(new ChangedEventArgs { Type = ProcessEventInfoType.Started, ProcessInfo = processInfo });
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

            RaiseChanged(new ChangedEventArgs { Type = ProcessEventInfoType.Stopped, ProcessInfo = processInfo });
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
