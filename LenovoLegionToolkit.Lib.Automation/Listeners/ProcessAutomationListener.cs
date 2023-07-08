using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Listeners;

public class ProcessAutomationListener : IListener<ProcessEventInfo>
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

    public event EventHandler<ProcessEventInfo>? Changed;

    private IDisposable? _processStartTraceDisposable;
    private IDisposable? _processStopTraceDisposable;

    private readonly Dictionary<int, ProcessInfo> _processCache = new();

    public Task StartAsync()
    {
        _processStartTraceDisposable = WMI.Win32.ProcessStartTrace.Listen(ProcessStartTraceListen_Handler);
        _processStopTraceDisposable = WMI.Win32.ProcessStopTrace.Listen(ProcessStopTraceListen_Handler);

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _processStartTraceDisposable?.Dispose();
        _processStopTraceDisposable?.Dispose();

        lock (Lock)
        {
            _processCache.Clear();
        }

        return Task.CompletedTask;
    }

    private void ProcessStartTraceListen_Handler(int processId, string processName)
    {
        lock (Lock)
        {
            if (processId < 0)
                return;

            if (string.IsNullOrWhiteSpace(processName))
                return;

            if (IgnoredNames.Contains(processName, StringComparer.InvariantCultureIgnoreCase))
                return;

            string? processPath = null;
            try
            {
                processPath = Process.GetProcessById(processId).GetFileName();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {processName} details.", ex);
            }

            if (!string.IsNullOrEmpty(processPath) && IgnoredPaths.Any(p => processPath.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var processInfo = new ProcessInfo(processName, processPath);
            _processCache[processId] = processInfo;

            Changed?.Invoke(this, new(ProcessEventInfoType.Started, processInfo));
        }
    }

    private void ProcessStopTraceListen_Handler(int processId, string processName)
    {
        lock (Lock)
        {
            CleanUpCacheIfNecessary();

            if (processId < 0)
                return;

            if (string.IsNullOrWhiteSpace(processName))
                return;

            if (IgnoredNames.Contains(processName, StringComparer.InvariantCultureIgnoreCase))
                return;

            if (!_processCache.TryGetValue(processId, out var processInfo))
                return;

            _processCache.Remove(processId);

            Changed?.Invoke(this, new(ProcessEventInfoType.Stopped, processInfo));
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
