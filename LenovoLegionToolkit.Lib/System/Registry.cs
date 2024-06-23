using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

// ReSharper disable StringLiteralTypo

namespace LenovoLegionToolkit.Lib.System;

public static class Registry
{
    public static IAsyncDisposable ObserveKey(string hive, string subKey, bool includeSubtreeChanges, Action handler)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var task = Task.Run(() => Handler(cancellationTokenSource.Token), cancellationTokenSource.Token);

        return new LambdaAsyncDisposable(async () =>
        {
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
            await task.ConfigureAwait(false);
        });

        void Handler(CancellationToken token)
        {
            try
            {
                using var baseKey = GetBaseKey(hive);
                using var key = baseKey.OpenSubKey(subKey) ?? throw new InvalidOperationException($"Key {subKey} could not be opened");

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

                    WaitHandle.WaitAny([resetEvent, token.WaitHandle]);
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
    }

    public static IDisposable ObserveValue(string hive, string path, string valueName, Action handler)
    {
        if (hive is "HKEY_CURRENT_USER" or "HKCU")
            hive = WindowsIdentity.GetCurrent().User?.Value ?? throw new InvalidOperationException("Current user value is null");

        var pathFormatted = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{hive}\\{path.Replace(@"\", @"\\")}' AND ValueName = '{valueName}'";

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting listener... [hive={hive}, pathFormatted={pathFormatted}, key={valueName}]");

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
            using var baseKey = GetBaseKey(hive);
            using var registryKey = baseKey.OpenSubKey(subKey);
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
        return baseKey.OpenSubKey(subKey)?.GetSubKeyNames().Select(s => Path.Combine(subKey, s)).ToArray() ?? [];
    }

    public static T GetValue<T>(string hive, string subKey, string valueName, T defaultValue, bool doNotExpand = false)
    {
        using var baseKey = GetBaseKey(hive);
        var value = baseKey.OpenSubKey(subKey)?.GetValue(valueName, defaultValue, doNotExpand ? RegistryValueOptions.DoNotExpandEnvironmentNames : RegistryValueOptions.None);

        if (value is not T t)
            return defaultValue;

        return t;
    }

    public static void SetValue<T>(string hive, string subKey, string valueName, T value, bool fixPermissions = false, RegistryValueKind valueKind = RegistryValueKind.Unknown) where T : notnull
    {
        try
        {
            Microsoft.Win32.Registry.SetValue(@$"{hive}\{subKey}", valueName, value, valueKind);
        }
        catch (UnauthorizedAccessException)
        {
            if (fixPermissions && AddPermissions(hive, subKey))
                SetValue(hive, subKey, valueName, value, false, valueKind);
            else
                throw;
        }
    }

    public static void Delete(string hive, string subKey)
    {
        using var baseKey = GetBaseKey(hive);
        using var key = baseKey.OpenSubKey(subKey);
        if (key is null)
            return;
        baseKey.DeleteSubKeyTree(subKey);
    }

    private static bool AddPermissions(string hive, string subKey)
    {
        IdentityReference? originalOwner = null;

        try
        {
            var current = WindowsIdentity.GetCurrent();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (current is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not get current user.");

                return false;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Attempting to add permissions to {hive}\\{subKey} for {current.Name}...");

            var user = current.User;
            if (user is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not get current security identifier of user {current.Name}.");

                return false;
            }

            if (!TakeOwnership(hive, subKey, user, out originalOwner))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Could not take ownership of {hive}\\{subKey}. [user={user}, originalOwner={originalOwner}]");

                return false;
            }

            using var baseKey = GetBaseKey(hive);
            using var key = baseKey.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions | RegistryRights.ReadKey);
            if (key is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to open key {hive}\\{subKey} for {current.Name}.");

                return false;
            }

            var accessControl = key.GetAccessControl();

            const RegistryRights rights = RegistryRights.FullControl;
            const AccessControlType type = AccessControlType.Allow;
            accessControl.AddAccessRule(new(user, rights, type));
            key.SetAccessControl(accessControl);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Permissions added on {hive}\\{subKey} for {current.Name}. [rights={rights}, type={type}]");

            return true;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to add permissions for {hive}\\{subKey}.", ex);

            throw;
        }
        finally
        {
            if (originalOwner is not null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Restoring ownership of {hive}\\{subKey} to {originalOwner}...");

                if (TakeOwnership(hive, subKey, originalOwner, out _))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Ownership of {hive}\\{subKey} restored to {originalOwner}.");
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Ownership of {hive}\\{subKey} NOT restored {originalOwner}.");
                }
            }
        }
    }

    private static bool TakeOwnership(string hive, string subKey, IdentityReference reference, out IdentityReference? previousIdentityReference)
    {
        previousIdentityReference = null;

        try
        {
            if (!TokenManipulator.AddPrivileges(TokenManipulator.SE_BACKUP_PRIVILEGE, TokenManipulator.SE_RESTORE_PRIVILEGE, TokenManipulator.SE_TAKE_OWNERSHIP_PRIVILEGE))
                return false;

            using var baseKey = GetBaseKey(hive);
            using var key = baseKey.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);
            if (key is null)
                return false;

            var accessControl = key.GetAccessControl();

            previousIdentityReference = accessControl.GetOwner(typeof(NTAccount));
            if (previousIdentityReference is null)
                return false;

            accessControl.SetOwner(reference);
            key.SetAccessControl(accessControl);

            return true;
        }
        finally
        {
            _ = TokenManipulator.RemovePrivileges(TokenManipulator.SE_BACKUP_PRIVILEGE, TokenManipulator.SE_RESTORE_PRIVILEGE, TokenManipulator.SE_TAKE_OWNERSHIP_PRIVILEGE);
        }
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
