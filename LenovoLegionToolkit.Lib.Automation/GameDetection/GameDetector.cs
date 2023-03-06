using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace LenovoLegionToolkit.Lib.Automation.GameDetection;

internal class GameDetector
{
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

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _listenTask;

    public Task StartAsync()
    {
        if (_listenTask is not null)
            return Task.CompletedTask;

        _cancellationTokenSource = new();
        _listenTask = Task.Run(() => Handler(_cancellationTokenSource.Token));

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        if (_listenTask != null)
            await _listenTask;
        _listenTask = null;
    }

    public static HashSet<ProcessInfo> GetDetectedGamePaths()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Finding detected games...");

        var result = new HashSet<ProcessInfo>();

        var key = Registry.CurrentUser.OpenSubKey(GAME_CONFIG_STORE_PATH);

        if (key is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't open {GAME_CONFIG_STORE_PATH} key.");

            return new();
        }

        var subKeyNames = key.GetSubKeyNames();

        foreach (var subKeyName in subKeyNames)
        {
            var exePath = key?.OpenSubKey(subKeyName)?.GetValue(MATCHED_EXE_FULL_PATH_KEY_NAME)?.ToString();
            if (exePath is null)
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

    private void Handler(CancellationToken token)
    {
        try
        {
            var lastPaths = GetDetectedGamePaths();

            var key = Registry.CurrentUser.OpenSubKey(GAME_CONFIG_STORE_PATH);
            if (key is null)
                throw new InvalidOperationException($"Key {GAME_CONFIG_STORE_PATH} could not be opened.");

            var resetEvent = new ManualResetEvent(false);

            while (true)
            {
                var regNotifyChangeKeyValueResult = PInvoke.RegNotifyChangeKeyValue(key.Handle,
                    true,
                    REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_LAST_SET | REG_NOTIFY_FILTER.REG_NOTIFY_THREAD_AGNOSTIC,
                    resetEvent.SafeWaitHandle,
                    true);
                if (regNotifyChangeKeyValueResult != WIN32_ERROR.NO_ERROR)
                    PInvokeExtensions.ThrowIfWin32Error("RegNotifyChangeKeyValue");

                WaitHandle.WaitAny(new[] { resetEvent, token.WaitHandle });
                token.ThrowIfCancellationRequested();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Games changed.");

                var newPaths = GetDetectedGamePaths();

                if (!newPaths.SetEquals(lastPaths))
                    GamesDetected?.Invoke(this, new(newPaths));

                lastPaths = newPaths;

                resetEvent.Reset();
            }
        }
        catch (OperationCanceledException) { }
        catch (ThreadAbortException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown error.", ex);
        }
    }
}
