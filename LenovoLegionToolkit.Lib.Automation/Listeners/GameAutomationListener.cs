using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.GameDetection;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Listeners;

public class GameAutomationListener : IListener<bool>
{
    private static readonly object _lock = new();

    public event EventHandler<bool>? Changed;

    private readonly GameDetector _gameDetector;
    private readonly InstanceEventListener _instanceCreationListener;
    private readonly InstanceEventListener _instanceDeletionListener;

    private readonly HashSet<string> _detectedGamePathsCache = new();
    private readonly HashSet<int> _runningGamesCache = new();

    public GameAutomationListener()
    {
        _gameDetector = new GameDetector();
        _gameDetector.GamesDetected += GameDetector_GamesDetected;

        _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
        _instanceCreationListener.Changed += InstanceCreationListener_Changed;

        _instanceDeletionListener = new InstanceEventListener(ProcessEventInfoType.Stopped, "Win32_ProcessStopTrace");
        _instanceDeletionListener.Changed += InstanceDeletionListener_Changed;
    }

    public async Task StartAsync()
    {
        lock (_lock)
        {
            foreach (var gamePath in GameDetector.GetDetectedGamePaths())
                _detectedGamePathsCache.Add(gamePath);
        }

        await _gameDetector.StartAsync().ConfigureAwait(false);

        await _instanceCreationListener.StartAsync().ConfigureAwait(false);
        await _instanceDeletionListener.StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        await _gameDetector.StopAsync().ConfigureAwait(false);

        await _instanceCreationListener.StopAsync().ConfigureAwait(false);
        await _instanceDeletionListener.StopAsync().ConfigureAwait(false);

        lock (_lock)
        {
            _detectedGamePathsCache.Clear();
            _runningGamesCache.Clear();
        }
    }

    private void GameDetector_GamesDetected(object? sender, GameDetector.GameDetectedEventArgs e)
    {
        lock (_lock)
        {
            foreach (var gamePath in e.GamePaths)
            {
                _detectedGamePathsCache.Add(gamePath);

                var filename = Path.GetFileNameWithoutExtension(gamePath);
                var processes = Process.GetProcessesByName(filename);

                foreach (var process in processes)
                {
                    try
                    {
                        var processPath = process.MainModule?.FileName;
                        if (!gamePath.Equals(processPath, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        _runningGamesCache.Add(process.Id);
                        Changed?.Invoke(this, true);
                        return;
                    }
                    catch (Exception)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Can't get process {gamePath} details, fallback to ID only.");
                    }
                }
            }
        }
    }

    private void InstanceCreationListener_Changed(object? sender, (ProcessEventInfoType type, int processID, string processName) e)
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(e.processName))
                return;

            if (e.processID < 0)
                return;

            string? processPath = null;
            try
            {
                processPath = Process.GetProcessById(e.processID).MainModule?.FileName;
            }
            catch (Exception)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {e.processName} details, fallback to ID only.");
            }

            if (processPath is null || !_detectedGamePathsCache.Contains(processPath))
                return;

            _runningGamesCache.Add(e.processID);

            Changed?.Invoke(this, true);
        }
    }


    private void InstanceDeletionListener_Changed(object? sender, (ProcessEventInfoType type, int processID, string processName) e)
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(e.processName))
                return;

            if (e.processID < 0)
                return;

            if (!_runningGamesCache.Contains(e.processID))
                return;

            _runningGamesCache.Remove(e.processID);

            Changed?.Invoke(this, false);
        }
    }
}