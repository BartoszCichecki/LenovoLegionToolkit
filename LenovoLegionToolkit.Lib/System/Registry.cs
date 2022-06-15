using System;
using System.Management;
using System.Security.Principal;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Registry
    {
        public static IDisposable Listen(string hive, string path, string key, Action handler)
        {
            if (hive == "HKEY_CURRENT_USER")
            {
                var currentUserValue = WindowsIdentity.GetCurrent()?.User?.Value;
                if (currentUserValue is null)
                    throw new InvalidOperationException("Current user value is null");
                hive = currentUserValue;
            }

            var pathFormatted = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{hive}\\{path.Replace(@"\", @"\\")}' AND ValueName = '{key}'";

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting listener... [hive={hive}, pathFormatted ={pathFormatted}, key={key}]");

            var watcher = new ManagementEventWatcher(pathFormatted);
            watcher.EventArrived += (s, e) =>
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event arrived [classPath={e.NewEvent.ClassPath}, hive={hive}, pathFormatted={pathFormatted}, key={key}]");

                handler();
            };
            watcher.Start();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started listener [hive={hive}, pathFormatted={pathFormatted}, key={key}]");

            return watcher;
        }

        public static T Read<T>(string hive, string path, string key, T defaultValue)
        {
            var result = Microsoft.Win32.Registry.GetValue(@$"{hive}\{path}", key, defaultValue);
            if (result is null)
                return defaultValue;
            return (T)result;
        }
    }
}
