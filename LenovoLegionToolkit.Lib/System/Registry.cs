using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace LenovoLegionToolkit.Lib.System;

public static class Registry
{
    public static IAsyncDisposable ObserveKey(string hive, string subKey, bool includeSubtreeChanges, Action handler)
    {
        void Handler(CancellationToken token)
        {
            try
            {
                using var baseKey = GetBaseKey(hive);
                using var key = baseKey.OpenSubKey(subKey) ?? throw new InvalidOperationException($"Key {subKey} could not be opened.");

                var resetEvent = new ManualResetEvent(false);

                while (true)
                {
                    var regNotifyChangeKeyValueResult = PInvoke.RegNotifyChangeKeyValue(key.Handle,
                        includeSubtreeChanges,
                        REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_LAST_SET | REG_NOTIFY_FILTER.REG_NOTIFY_THREAD_AGNOSTIC,
                        resetEvent.SafeWaitHandle,
                        true);
                    if (regNotifyChangeKeyValueResult != WIN32_ERROR.NO_ERROR)
                        PInvokeExtensions.ThrowIfWin32Error("RegNotifyChangeKeyValue");

                    WaitHandle.WaitAny(new[] { resetEvent, token.WaitHandle });
                    token.ThrowIfCancellationRequested();

                    handler();

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

        var cancellationTokenSource = new CancellationTokenSource();
        var task = Task.Run(() => Handler(cancellationTokenSource.Token), cancellationTokenSource.Token);

        return new LambdaAsyncDisposable(async () =>
        {
            cancellationTokenSource.Cancel();
            await task;
        });
    }

    public static IDisposable ObserveValue(string hive, string path, string valueName, Action handler)
    {
        if (hive is "HKEY_CURRENT_USER" or "HKCU")
            hive = WindowsIdentity.GetCurrent().User?.Value ?? throw new InvalidOperationException("Current user value is null");

        var pathFormatted = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{hive}\\{path.Replace(@"\", @"\\")}' AND ValueName = '{valueName}'";

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting listener... [hive={hive}, pathFormatted ={pathFormatted}, key={valueName}]");

        var watcher = new ManagementEventWatcher(pathFormatted);
        watcher.EventArrived += (_, e) =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event arrived [classPath={e.NewEvent.ClassPath}, hive={hive}, pathFormatted={pathFormatted}, key={valueName}]");

            handler();
        };
        watcher.Start();

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Started listener [hive={hive}, pathFormatted={pathFormatted}, key={valueName}]");

        return watcher;
    }

    public static bool KeyExists(string hive, string subKey)
    {
        try
        {
            using var registryKey = GetBaseKey(hive).OpenSubKey(subKey);
            return registryKey is not null;
        }
        catch
        {
            return false;
        }
    }

    public static bool ValueExists(string hive, string subKey, string valueName)
    {
        try
        {
            var keyName = Path.Combine(hive, subKey);
            var value = Microsoft.Win32.Registry.GetValue(keyName, valueName, null);
            return value is not null;
        }
        catch
        {
            return false;
        }
    }

    public static string[] GetSubKeys(string hive, string subKey)
    {
        using var baseKey = GetBaseKey(hive);
        return baseKey.OpenSubKey(subKey)?.GetSubKeyNames().Select(s => Path.Combine(subKey, s)).ToArray() ?? Array.Empty<string>();
    }

    public static T GetValue<T>(string hive, string subKey, string valueName, T defaultValue)
    {
        var keyName = Path.Combine(hive, subKey);
        var result = Microsoft.Win32.Registry.GetValue(keyName, valueName, defaultValue);
        if (result is null)
            return defaultValue;
        return (T)result;
    }

    public static void SetValue<T>(string hive, string subKey, string valueName, T value) where T : notnull
    {
        Microsoft.Win32.Registry.SetValue(@$"{hive}\{subKey}", valueName, value);
    }

    private static RegistryKey GetBaseKey(string hive) => hive switch
    {
        "HKLM" or "HKEY_LOCAL_MACHINE" => Microsoft.Win32.Registry.LocalMachine,
        "HKCU" or "HKEY_CURRENT_USER" => Microsoft.Win32.Registry.CurrentUser,
        "HKU" or "HKEY_USERS" => Microsoft.Win32.Registry.Users,
        "HKCR" or "HKEY_CLASSES_ROOT " => Microsoft.Win32.Registry.ClassesRoot,
        "HKCC" or "HKEY_CURRENT_CONFIG  " => Microsoft.Win32.Registry.CurrentConfig,
        _ => throw new ArgumentException(@"Unknown hive.", nameof(hive))
    };
}