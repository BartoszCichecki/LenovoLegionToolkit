using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.GameDetection;

internal class GameDetector
{
    private const string GAME_CONFIG_STORE_HIVE = "HKEY_CURRENT_USER";
    private const string GAME_CONFIG_STORE_PATH = @"System\GameConfigStore\Children";
    private const string MATCHED_EXE_FULL_PATH_KEY_NAME = "MatchedExeFullPath";

    public class GameDetectedEventArgs : EventArgs
    {
        public HashSet<ProcessInfo> Games { get; }

        public GameDetectedEventArgs(HashSet<ProcessInfo> games)
        {
            Games = games;
        }
    }

    public event EventHandler<GameDetectedEventArgs>? GamesDetected;

    private IAsyncDisposable? _listener;

    public Task StartAsync()
    {
        if (_listener is not null)
            return Task.CompletedTask;

        var lastPaths = GetDetectedGamePaths();

        _listener = Registry.ObserveKey(GAME_CONFIG_STORE_HIVE, GAME_CONFIG_STORE_PATH, true, () =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Registry changed...");

            var newPaths = GetDetectedGamePaths();

            if (!newPaths.SetEquals(lastPaths))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Detected games changed.");

                GamesDetected?.Invoke(this, new(newPaths));
            }

            lastPaths = newPaths;
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_listener != null)
            await _listener.DisposeAsync();
        _listener = null;
    }

    public static HashSet<ProcessInfo> GetDetectedGamePaths()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Detecting games...");

        var result = new HashSet<ProcessInfo>();

        foreach (var subKey in Registry.GetSubKeys(GAME_CONFIG_STORE_HIVE, GAME_CONFIG_STORE_PATH))
        {
            var exePath = Registry.GetValue(GAME_CONFIG_STORE_HIVE, subKey, MATCHED_EXE_FULL_PATH_KEY_NAME, string.Empty);
            if (string.IsNullOrEmpty(exePath))
                continue;

            result.Add(ProcessInfo.FromPath(exePath));
        }

        if (Log.Instance.IsTraceEnabled)
        {
            Log.Instance.Trace($"Detected games:");
            foreach (var r in result)
                Log.Instance.Trace($" - {r}");
        }

        return result;
    }
}
