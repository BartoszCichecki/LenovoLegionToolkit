using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class WMI
    {
        public static IDisposable Listen(string scope, string query, Action<PropertyDataCollection> handler)
        {
            var watcher = new ManagementEventWatcher(scope, query);
            watcher.EventArrived += (s, e) => handler(e.NewEvent.Properties);
            watcher.Start();
            return watcher;
        }

        public static IEnumerable<T> Read<T>(string scope, string query, Func<PropertyDataCollection, T> converter)
        {
            using var searcher = new ManagementObjectSearcher(scope, query);
            foreach (var queryObj in searcher.Get())
                yield return converter(queryObj.Properties);
        }

        public static void Invoke(string scope,
            string clazz,
            string propertyName,
            string propertyValue,
            string methodName,
            object[] parameters = null)
        {
            var path = $"{scope}:{clazz}.{propertyName}=\"{propertyValue.Escaped()}\"";

            // Invoke the async version.
            // Synchronous seems to throw a NRE, for no reason.
            using var mre = new ManualResetEvent(false);
            var obs = new ManagementOperationObserver();
            obs.Completed += (s, e) => mre.Set();
            using var managementObject = new ManagementObject(path);
            managementObject.InvokeMethod(obs, methodName, parameters);
            mre.WaitOne();
        }

        private static string Escaped(this string value) => value.Replace("\\", "\\\\");
    }
}
