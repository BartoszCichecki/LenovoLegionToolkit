using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace LenovoLegionToolkit.Lib.GameDetection;

public class GameDetector
{
    private const string GAME_CONFIG_STORE_PATH = @"System\GameConfigStore\Children";
    private const string MATCHED_EXE_FULL_PATH_KEY_NAME = "MatchedExeFullPath";

    public class GameDetectedEventArgs : EventArgs
    {
        public HashSet<string> Paths { get; }

        public GameDetectedEventArgs(HashSet<string> paths)
        {
            Paths = paths;
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

    public HashSet<string> GetDetectedGamePaths()
    {
        var result = new HashSet<string>();

        var key = Registry.CurrentUser.OpenSubKey(GAME_CONFIG_STORE_PATH);
        var subKeyNames = key?.GetSubKeyNames() ?? Array.Empty<string>();

        foreach (var subKeyName in subKeyNames)
        {
            var exePath = key?.OpenSubKey(subKeyName)?.GetValue(MATCHED_EXE_FULL_PATH_KEY_NAME)?.ToString();
            if (exePath is null)
                continue;

            result.Add(exePath);
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

                var newPaths = GetDetectedGamePaths();

                if (newPaths.Except(lastPaths, StringComparer.CurrentCultureIgnoreCase).Any())
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