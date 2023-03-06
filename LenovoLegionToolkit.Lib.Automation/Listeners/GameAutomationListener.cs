using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    private readonly HashSet<ProcessInfo> _detectedGamePathsCache = new();
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
            _detectedGamePathsCache.Clear();
            foreach (var game in e.Games)
                _detectedGamePathsCache.Add(game);

            var foundRunning = false;

            foreach (var game in e.Games)
            {
                foreach (var process in Process.GetProcessesByName(game.Name))
                {
                    try
                    {
                        var processPath = process.MainModule?.FileName;
                        if (game.ExecutablePath is null || !game.ExecutablePath.Equals(processPath, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        _runningGamesCache.Add(process.Id);
                        foundRunning = true;
                    }
                    catch (Exception)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Can't get game \"{game}\" details.");
                    }
                }
            }

            if (!foundRunning)
                return;

            Changed?.Invoke(this, true);
        }
    }

    private void InstanceCreationListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
    {
        lock (_lock)
        {
            if (e.processId < 0)
                return;

            if (!_detectedGamePathsCache.Any(p => e.processName.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase)))
                return;

            string? processPath = null;
            try
            {
                processPath = Process.GetProcessById(e.processId).MainModule?.FileName;
            }
            catch (Exception)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't get process {e.processName} details.");
            }

            if (processPath is null)
                return;

            var processInfo = ProcessInfo.FromPath(processPath);
            if (!_detectedGamePathsCache.Contains(processInfo))
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Game {processInfo} is running. [processId={e.processId}]");

            _runningGamesCache.Add(e.processId);

            if (_runningGamesCache.Count > 1)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Notify for game {processInfo} skipped. Total of {_runningGamesCache.Count} games are running. [processId={e.processId}]");

                return;
            }

            Changed?.Invoke(this, true);
        }
    }


    private void InstanceDeletionListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
    {
        lock (_lock)
        {
            if (e.processId < 0)
                return;

            if (!_runningGamesCache.Remove(e.processId))
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Game with process ID {e.processId} stopped.");

            if (_runningGamesCache.Count > 0)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Notify for game with process ID {e.processId} skipped. Total of {_runningGamesCache.Count} games are running. [processId={e.processId}]");

                return;
            }

            Changed?.Invoke(this, false);
        }
    }
}