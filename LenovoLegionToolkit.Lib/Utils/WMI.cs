using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal static class WMI
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

        public static Task<IEnumerable<T>> ReadAsync<T>(string scope, FormattableString query, Func<PropertyDataCollection, T> converter)
        {
            return Task.Run<IEnumerable<T>>(() =>
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
                return stringArg ?? string.Empty;
            }
        }
    }
}
