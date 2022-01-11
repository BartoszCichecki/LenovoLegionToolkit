using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractWmiFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly string _methodNameSuffix;
        private readonly int _offset;
        private readonly string? _supportMethodName;
        private readonly int _supportOffset;

        protected AbstractWmiFeature(string methodNameSuffix, int offset, string? supportMethodName = null, int supportOffset = 0)
        {
            _methodNameSuffix = methodNameSuffix;
            _offset = offset;
            _supportMethodName = supportMethodName;
            _supportOffset = supportOffset;
        }

        public virtual async Task<T> GetStateAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");

            if (!await IsSupportedAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Feature {_methodNameSuffix} is not supported [feature={GetType().Name}]");

                throw new NotSupportedException($"Feature {_methodNameSuffix} is not supported.");
            }

            var result = FromInternal(await ExecuteGamezoneAsync("Get" + _methodNameSuffix, "Data").ConfigureAwait(false));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");

            return result;
        }

        public virtual Task SetStateAsync(T state)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");

            ExecuteGamezoneAsync("Set" + _methodNameSuffix, "Data",
                new Dictionary<string, string>
                {
                    {"Data", ToInternal(state).ToString()}
                });

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");

            return Task.CompletedTask;
        }

        private async Task<bool> IsSupportedAsync()
        {
            if (_supportMethodName == null)
                return true;

            var value = await ExecuteGamezoneAsync(_supportMethodName, "Data").ConfigureAwait(false);
            return value > _supportOffset;
        }

        private int ToInternal(T state) => (int)(object)state + _offset;

        private T FromInternal(int state) => (T)(object)(state - _offset);

        private Task<int> ExecuteGamezoneAsync(string methodName, string resultPropertyName, Dictionary<string, string>? methodParams = null)
        {
            return ExecuteAsync("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);
        }

        private Task<int> ExecuteAsync(string queryString,
            string methodName,
            string resultPropertyName,
            Dictionary<string, string>? methodParams = null)
        {
            return Task.Run(() =>
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Executing WMI query... [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

                using var enumerator = new ManagementObjectSearcher("ROOT\\WMI", queryString).Get().GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"No results in query [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

                    throw new InvalidOperationException("No results in query");
                }

                var mo = (ManagementObject)enumerator.Current;
                var methodParamsObject = mo.GetMethodParameters(methodName);
                if (methodParams != null)
                    foreach (var pair in methodParams)
                        methodParamsObject[pair.Key] = pair.Value;

                var result = mo.InvokeMethod(methodName, methodParamsObject, null)?.Properties[resultPropertyName].Value;
                var intResult = Convert.ToInt32(result);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Executed WMI query with result {intResult} [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

                return intResult;
            });
        }
    }
}