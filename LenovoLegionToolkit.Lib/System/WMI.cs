using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.System
{
    public static class WMI
    {
        public static IDisposable Listen(string scope, FormattableString query, Action<PropertyDataCollection> handler)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);
            var watcher = new ManagementEventWatcher(scope, queryFormatted);
            watcher.EventArrived += (s, e) => handler(e.NewEvent.Properties);
            watcher.Start();
            return watcher;
        }

        public static async Task<IEnumerable<T>> ReadAsync<T>(string scope, FormattableString query, Func<PropertyDataCollection, T> converter)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);
            var mos = new ManagementObjectSearcher(scope, queryFormatted);
            var managementObjects = await mos.GetAsync().ConfigureAwait(false);
            var result = managementObjects.Select(mo => mo.Properties).Select(converter);
            return result;
        }

        public static async Task CallAsync(string scope, FormattableString query, string methodName, Dictionary<string, object> methodParams)
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);
            var mos = new ManagementObjectSearcher(scope, queryFormatted);
            var managementObjects = await mos.GetAsync().ConfigureAwait(false);
            var managementObject = managementObjects.FirstOrDefault();

            if (managementObject is null)
                throw new InvalidOperationException("No results in query");

            var mo = (ManagementObject)managementObject;
            var methodParamsObject = mo.GetMethodParameters(methodName);
            foreach (var pair in methodParams)
                methodParamsObject[pair.Key] = pair.Value;

            mo.InvokeMethod(methodName, methodParamsObject, null);
        }

        public static async Task<T> CallAsync<T>(string scope, FormattableString query, string methodName, Dictionary<string, object> methodParams, Func<PropertyDataCollection, T> converter) where T : struct
        {
            var queryFormatted = query.ToString(WMIPropertyValueFormatter.Instance);

            var mos = new ManagementObjectSearcher(scope, queryFormatted);
            var managementObjects = await mos.GetAsync().ConfigureAwait(false);
            var managementObject = managementObjects.FirstOrDefault();

            if (managementObject is null)
                throw new InvalidOperationException("No results in query");

            var mo = (ManagementObject)managementObject;
            var methodParamsObject = mo.GetMethodParameters(methodName);
            foreach (var pair in methodParams)
                methodParamsObject[pair.Key] = pair.Value;

            var resultProperties = mo.InvokeMethod(methodName, methodParamsObject, null);

            if (resultProperties is null)
                return default;

            var result = converter(resultProperties.Properties);
            return result;
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
                var stringArg = arg?.ToString()?.Replace("\\", "\\\\");
                return stringArg ?? "";
            }
        }
    }
}