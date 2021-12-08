using System;
using System.Collections.Generic;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class WMI
    {
        public static IDisposable Listen(string scope, FormattableString query, Action<PropertyDataCollection> handler)
        {
            var watcher = new ManagementEventWatcher(scope, query.ToString(WMIPropertyValueFormatter.Instance));
            watcher.EventArrived += (s, e) => handler(e.NewEvent.Properties);
            watcher.Start();
            return watcher;
        }

        public static IEnumerable<T> Read<T>(string scope, FormattableString query, Func<PropertyDataCollection, T> converter)
        {
            using var searcher = new ManagementObjectSearcher(scope, query.ToString(WMIPropertyValueFormatter.Instance));
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
            var path = $"{scope}:{clazz}.{propertyName}='{propertyValue.ToString(WMIPropertyValueFormatter.Instance)}'";

            using var managementObject = new ManagementObject(path);
            return managementObject.InvokeMethod(methodName, parameters);
        }

        private class WMIPropertyValueFormatter : IFormatProvider, ICustomFormatter
        {
            public static readonly WMIPropertyValueFormatter Instance = new();

            private WMIPropertyValueFormatter() { }

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;

                return null;
            }
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                var stringArg = arg.ToString();
                stringArg = stringArg.Replace("\\", "\\\\");
                return stringArg;
            }
        }
    }
}
