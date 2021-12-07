using System;
using System.Collections.Generic;
using System.Management;

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

        public static object Invoke(string scope,
            string clazz,
            string propertyName,
            string propertyValue,
            string methodName,
            object[] parameters = null)
        {
            var path = $"{scope}:{clazz}.{propertyName}=\"{propertyValue.Escaped()}\"";

            using var managementObject = new ManagementObject(path);
            return managementObject.InvokeMethod(methodName, parameters);
        }

        private static string Escaped(this string value) => value.Replace("\\", "\\\\");
    }
}
