using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.GameDetection;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class GameAutoListener : AbstractAutoListener<GameAutoListener.ChangedEventArgs>
{
    public class ChangedEventArgs(bool running) : EventArgs
    {
        public bool Running { get; } = running;
    }

    private class ProcessEqualityComparer : IEqualityComparer<Process>
    {
        public bool Equals(Process? x, Process? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Process obj) => obj.Id;
    }

    private static readonly object Lock = new();

    private readonly InstanceStartedEventAutoAutoListener _instanceStartedEventAutoAutoListener;

    private readonly GameConfigStoreDetector _gameConfigStoreDetector;
    private readonly EffectiveGameModeDetector _effectiveGameModeDetector;

    private readonly HashSet<ProcessInfo> _detectedGamePathsCache = [];
    private readonly HashSet<Process> _processCache = new(new ProcessEqualityComparer());

    private bool _lastState;

    public GameAutoListener(InstanceStartedEventAutoAutoListener instanceStartedEventAutoAutoListener)
    {
        _instanceStartedEventAutoAutoListener = instanceStartedEventAutoAutoListener;

        _gameConfigStoreDetector = new GameConfigStoreDetector();
        _gameConfigStoreDetector.GamesDetected += GameConfigStoreDetectorGamesConfigStoreDetected;

        _effectiveGameModeDetector = new EffectiveGameModeDetector();
        _effectiveGameModeDetector.Changed += EffectiveGameModeDetectorChanged;
    }

    protected override async Task StartAsync()
    {
        lock (Lock)
        {
            foreach (var gamePath in GameConfigStoreDetector.GetDetectedGamePaths())
                _detectedGamePathsCache.Add(gamePath);
        }

        await _gameConfigStoreDetector.StartAsync().ConfigureAwait(false);
        await _effectiveGameModeDetector.StartAsync().ConfigureAwait(false);

        await _instanceStartedEventAutoAutoListener.SubscribeChangedAsync(InstanceStartedEventAutoAutoListener_Changed).ConfigureAwait(false);
    }

    protected override async Task StopAsync()
    {
        await _instanceStartedEventAutoAutoListener.UnsubscribeChangedAsync(InstanceStartedEventAutoAutoListener_Changed).ConfigureAwait(false);

        await _gameConfigStoreDetector.StopAsync().ConfigureAwait(false);
        await _effectiveGameModeDetector.StopAsync().ConfigureAwait(false);

        lock (Lock)
        {
            foreach (var process in _processCache)
                Detach(process);

            _processCache.Clear();
            _detectedGamePathsCache.Clear();
            _lastState = false;
        }
    }

    public bool AreGamesRunning()
    {
        lock (Lock)
        {
            return _lastState;
        }
    }

    private void GameConfigStoreDetectorGamesConfigStoreDetected(object? sender, GameConfigStoreDetector.GameDetectedEventArgs e)
    {
        lock (Lock)
        {
            _detectedGamePathsCache.Clear();

            foreach (var game in e.Games)
            {
                _detectedGamePathsCache.Add(game);

                foreach (var process in Process.GetProcessesByName(game.Name))
                {
                    try
                    {
                        var processPath = process.MainModule?.FileName;
                        if (game.ExecutablePath is null || !game.ExecutablePath.Equals(processPath, StringComparison.CurrentCultureIgnoreCase))
                            continue;

                        if (!_processCache.Contains(process))
                        {
                            Attach(process);
                            _processCache.Add(process);
                        }

                        RaiseChangedIfNeeded(true);
                    }
                    catch (Exception)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Can't get game \"{game}\" details.");
                    }
                }
            }
        }
    }

    private void EffectiveGameModeDetectorChanged(object? sender, bool e)
    {
        lock (Lock)
        {
            if (_processCache.Count != 0)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, process cache is not empty.");
                return;
            }

            RaiseChangedIfNeeded(e);
        }
    }

    private void InstanceStartedEventAutoAutoListener_Changed(object? sender, InstanceStartedEventAutoAutoListener.ChangedEventArgs e)
    {
        lock (Lock)
        {
            if (e.ProcessId < 0)
                return;

            if (!_detectedGamePathsCache.Any(p => e.ProcessName.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase)))
                return;

            try
            {
                var process = Process.GetProcessById(e.ProcessId);
                var processPath = process.GetFileName();

                if (string.IsNullOrEmpty(processPath))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Can't get path for {e.ProcessName}. [processId={e.ProcessId}]");

                    return;
                }

                var processInfo = ProcessInfo.FromPath(processPath);
                if (!_detectedGamePathsCache.Contains(processInfo))
                    return;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Game {processInfo} is running. [processId={e.ProcessId}, processPath={processPath}]");

                Attach(process);
                _processCache.Add(process);

                RaiseChangedIfNeeded(true);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to attach to {e.ProcessName}. [processId={e.ProcessId}]", ex);
            }
        }
    }

    private void RaiseChangedIfNeeded(bool newState)
    {
        lock (Lock)
        {
            if (newState == _lastState)
                return;

            _lastState = newState;

            RaiseChanged(new ChangedEventArgs(newState));
        }
    }

    private void Attach(Process process)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Attaching to process {process.Id}...");

        process.EnableRaisingEvents = true;
        process.Exited += Process_Exited;
    }

    private void Detach(Process process)
    {
        process.EnableRaisingEvents = false;
        process.Exited -= Process_Exited;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Detached from process {process.Id}.");
    }

    private void Process_Exited(object? o, EventArgs args)
    {
        lock (Lock)
        {
            if (o is not Process process)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Process {process.Id} exited.");

            var staleProcesses = _processCache.RemoveWhere(p =>
            {
                try { return p.HasExited; }
                catch { return true; }
            });

            if (staleProcesses > 1)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Removed {staleProcesses} stale processes.");
            }

            if (_processCache.Count != 0)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"More games are running...");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"No more games are running.");

            RaiseChangedIfNeeded(false);
        }
    }
}
