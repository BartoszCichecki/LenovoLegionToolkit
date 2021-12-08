using System;
using System.Collections.Generic;
using System.Management;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class WMI
    {
        public static IDisposable Listen(string scope, FormattableString query, Action<PropertyDataCollection> handler)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            Log.Instance.Trace($"Starting listener... [scope={scope}, queryFormatted={queryFormatted}]");

            var watcher = new ManagementEventWatcher(scope, queryFormatted);
            watcher.EventArrived += (s, e) =>
            {
                Log.Instance.Trace($"Event arrived [classPath={e.NewEvent.ClassPath}, scope={scope}, queryFormatted={queryFormatted}]");

                handler(e.NewEvent.Properties);
            };
            watcher.Start();

            Log.Instance.Trace($"Started listener [scope={scope}, queryFormatted={queryFormatted}]");

            return watcher;
        }

        public static IEnumerable<T> Read<T>(string scope, FormattableString query, Func<PropertyDataCollection, T> converter)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            Log.Instance.Trace($"Reading... [scope={scope}, queryFormatted={queryFormatted}]");

            using var searcher = new ManagementObjectSearcher(scope, queryFormatted);
            foreach (var queryObj in searcher.Get())
                yield return converter(queryObj.Properties);

            Log.Instance.Trace($"Read [scope={scope}, queryFormatted={queryFormatted}]");
        }

        public static object Invoke(string scope,
            string clazz,
            string propertyName,
            string propertyValue,
            string methodName,
            object[] parameters = null)
        {
            var path = $"{scope}:{clazz}.{propertyName}='{propertyValue.ToString(WMIPropertyValueFormatter.Instance)}'";

            Log.Instance.Trace($"Invoking... [path={path}]");

            using var managementObject = new ManagementObject(path);
            var result = managementObject.InvokeMethod(methodName, parameters);

            Log.Instance.Trace($"Invoked [path={path}, result={result}]");

            return result;
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
