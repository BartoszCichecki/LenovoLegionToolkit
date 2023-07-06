﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.GameDetection;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Listeners;

public class GameAutomationListener : IListener<bool>
{
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

    public event EventHandler<bool>? Changed;

    private readonly GameDetector _gameDetector;
    private readonly InstanceEventListener _instanceCreationListener;
    private readonly EffectiveGameModeListener _effectiveGameModeListener;

    private readonly HashSet<ProcessInfo> _detectedGamePathsCache = new();
    private readonly HashSet<Process> _processCache = new(new ProcessEqualityComparer());

    private bool _lastState;

    public GameAutomationListener()
    {
        _gameDetector = new GameDetector();
        _gameDetector.GamesDetected += GameDetector_GamesDetected;

        _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
        _instanceCreationListener.Changed += InstanceCreationListener_Changed;

        _effectiveGameModeListener = new EffectiveGameModeListener();
        _effectiveGameModeListener.Changed += EffectiveGameModeListenerChanged;
    }

    public async Task StartAsync()
    {
        lock (Lock)
        {
            foreach (var gamePath in GameDetector.GetDetectedGamePaths())
                _detectedGamePathsCache.Add(gamePath);
        }

        await _gameDetector.StartAsync().ConfigureAwait(false);

        await _instanceCreationListener.StartAsync().ConfigureAwait(false);
        await _effectiveGameModeListener.StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        await _gameDetector.StopAsync().ConfigureAwait(false);

        await _instanceCreationListener.StopAsync().ConfigureAwait(false);
        await _effectiveGameModeListener.StopAsync().ConfigureAwait(false);

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

    private void GameDetector_GamesDetected(object? sender, GameDetector.GameDetectedEventArgs e)
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

    private void InstanceCreationListener_Changed(object? sender, (ProcessEventInfoType type, int processId, string processName) e)
    {
        lock (Lock)
        {
            if (e.processId < 0)
                return;

            if (!_detectedGamePathsCache.Any(p => e.processName.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase)))
                return;

            try
            {
                var process = Process.GetProcessById(e.processId);
                var processPath = process.GetFileName();

                if (string.IsNullOrEmpty(processPath))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Can't get path for {e.processName}. [processId={e.processId}]");

                    return;
                }

                var processInfo = ProcessInfo.FromPath(processPath);
                if (!_detectedGamePathsCache.Contains(processInfo))
                    return;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Game {processInfo} is running. [processId={e.processId}, processPath={processPath}]");

                Attach(process);
                _processCache.Add(process);

                RaiseChangedIfNeeded(true);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to attach to {e.processName}. [processId={e.processId}]", ex);
            }
        }
    }

    private void EffectiveGameModeListenerChanged(object? sender, bool e)
    {
        lock (Lock)
        {
            if (_processCache.Any())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, process cache is not empty.");
                return;
            }

            RaiseChangedIfNeeded(e);
        }
    }

    private void RaiseChangedIfNeeded(bool newState)
    {
        lock (Lock)
        {
            if (newState == _lastState)
                return;

            _lastState = newState;

            Changed?.Invoke(this, newState);
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

            var staleProcesses = _processCache.RemoveWhere(p => p.HasExited);
            if (staleProcesses > 1)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Removed {staleProcesses} stale processes.");
            }

            if (_processCache.Any())
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
