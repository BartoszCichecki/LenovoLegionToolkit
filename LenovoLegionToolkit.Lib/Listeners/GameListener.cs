using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.GameDetection;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class GameListener : IListener<bool>
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

    private readonly GameConfigStoreDetector _gameConfigStoreDetector;
    private readonly EffectiveGameModeDetector _effectiveGameModeDetector;
    private readonly InstanceEventListener _instanceCreationListener;

    private readonly HashSet<ProcessInfo> _detectedGamePathsCache = new();
    private readonly HashSet<Process> _processCache = new(new ProcessEqualityComparer());

    private bool _lastState;

    public GameListener()
    {
        _gameConfigStoreDetector = new GameConfigStoreDetector();
        _gameConfigStoreDetector.GamesDetected += GameConfigStoreDetectorGamesConfigStoreDetected;

        _effectiveGameModeDetector = new EffectiveGameModeDetector();
        _effectiveGameModeDetector.Changed += EffectiveGameModeDetectorChanged;

        _instanceCreationListener = new InstanceEventListener(ProcessEventInfoType.Started, "Win32_ProcessStartTrace");
        _instanceCreationListener.Changed += InstanceCreationListener_Changed;
    }

    public async Task StartAsync()
    {
        lock (Lock)
        {
            foreach (var gamePath in GameConfigStoreDetector.GetDetectedGamePaths())
                _detectedGamePathsCache.Add(gamePath);
        }

        await _gameConfigStoreDetector.StartAsync().ConfigureAwait(false);
        await _effectiveGameModeDetector.StartAsync().ConfigureAwait(false);

        await _instanceCreationListener.StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        await _gameConfigStoreDetector.StopAsync().ConfigureAwait(false);
        await _effectiveGameModeDetector.StopAsync().ConfigureAwait(false);

        await _instanceCreationListener.StopAsync().ConfigureAwait(false);

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
            if (_processCache.Any())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, process cache is not empty.");
                return;
            }

            RaiseChangedIfNeeded(e);
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
