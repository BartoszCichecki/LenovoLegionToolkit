using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.System
{
    public static class WMI
    {
        public static IDisposable Listen(string scope, FormattableString query, Action<PropertyDataCollection> handler)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting listener... [scope={scope}, queryFormatted={queryFormatted}]");

            var watcher = new ManagementEventWatcher(scope, queryFormatted);
            watcher.EventArrived += (s, e) =>
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event arrived [classPath={e.NewEvent.ClassPath}, scope={scope}, queryFormatted={queryFormatted}]");

                handler(e.NewEvent.Properties);
            };
            watcher.Start();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started listener [scope={scope}, queryFormatted={queryFormatted}]");

            return watcher;
        }

        public static Task<IEnumerable<T>> ReadAsync<T>(string scope, FormattableString query, Func<PropertyDataCollection, T> converter) => Task.Run<IEnumerable<T>>(() =>
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Reading... [scope={scope}, queryFormatted={queryFormatted}]");

            var result = new List<T>();

            using var searcher = new ManagementObjectSearcher(scope, queryFormatted);
            foreach (var queryObj in searcher.Get())
                result.Add(converter(queryObj.Properties));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Read [scope={scope}, queryFormatted={queryFormatted}]");

            return result;
        });

        public static Task CallAsync(string scope, FormattableString query, string methodName, Dictionary<string, object> methodParams) => Task.Run(() =>
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Writing... [scope={scope}, queryFormatted={queryFormatted}, methodName={methodName}]");

            using var enumerator = new ManagementObjectSearcher(scope, queryFormatted).Get().GetEnumerator();
            if (!enumerator.MoveNext())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"No results in query [queryFormatted={queryFormatted}, methodName={methodName}]");

                throw new InvalidOperationException("No results in query");
            }

            var mo = (ManagementObject)enumerator.Current;
            var methodParamsObject = mo.GetMethodParameters(methodName);
            foreach (var pair in methodParams)
                methodParamsObject[pair.Key] = pair.Value;

            mo.InvokeMethod(methodName, methodParamsObject, null);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Write successful. [queryFormatted={queryFormatted}, methodName={methodName}, methodParams.Count={methodParams?.Count}]");
        });

        public static Task<T> CallAsync<T>(string scope, FormattableString query, string methodName, Dictionary<string, object> methodParams, Func<PropertyDataCollection, T> converter) where T : struct
        {
            return Task.Run(() =>
            {
                var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Writing... [scope={scope}, queryFormatted={queryFormatted}, methodName={methodName}]");

                using var enumerator = new ManagementObjectSearcher(scope, queryFormatted).Get().GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"No results in query [queryFormatted={queryFormatted}, methodName={methodName}]");

                    throw new InvalidOperationException("No results in query");
                }

                var mo = (ManagementObject)enumerator.Current;
                var methodParamsObject = mo.GetMethodParameters(methodName);
                foreach (var pair in methodParams)
                    methodParamsObject[pair.Key] = pair.Value;

                var resultProperties = mo.InvokeMethod(methodName, methodParamsObject, null);

                if (resultProperties is null)
                    return default;

                var result = converter(resultProperties.Properties);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Write successful. [queryFormatted={queryFormatted}, methodName={methodName}, methodParams.Count={methodParams?.Count}]");

                return result;
            });
        }

        private class WMIPropertyValueFormatter : IFormatProvider, ICustomFormatter
        {
            public static readonly WMIPropertyValueFormatter Instance = new();

            private WMIPropertyValueFormatter() { }

            public object GetFormat(Type? formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;

                throw new InvalidOperationException("Invalid type of formatted");
            }

            public string Format(string? format, object? arg, IFormatProvider? formatProvider)
            {
                var stringArg = arg?.ToString();
                stringArg = stringArg?.Replace("\\", "\\\\");
                return stringArg ?? "";
            }
        }
    }
}
