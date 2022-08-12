using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractLenovoGamezoneWmiFeature<T> : IFeature<T> where T : struct, Enum, IComparable
    {
        private readonly string _methodNameSuffix;
        private readonly int _offset;
        private readonly string? _supportMethodName;
        private readonly int _supportOffset;
        private readonly string? _getMethodNameSuffix;

        protected AbstractLenovoGamezoneWmiFeature(string methodNameSuffix, int offset, string? supportMethodName = null, int supportOffset = 0, string? getMethodNameSuffix = null)
        {
            _methodNameSuffix = methodNameSuffix;
            _offset = offset;
            _supportMethodName = supportMethodName;
            _supportOffset = supportOffset;
            _getMethodNameSuffix = getMethodNameSuffix;
        }

        public async Task<bool> IsSupportedAsync()
        {
            try
            {
                if (_supportMethodName is null)
                    return true;

                var value = await ExecuteGamezoneAsync(_supportMethodName, "Data").ConfigureAwait(false);
                return value > _supportOffset;
            }
            catch
            {
                return false;
            }
        }

        public virtual Task<T[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<T>());

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

            var result = FromInternal(await ExecuteGamezoneAsync("Get" + (_getMethodNameSuffix ?? _methodNameSuffix), "Data").ConfigureAwait(false));

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

        private int ToInternal(T state) => (int)(object)state + _offset;

        private T FromInternal(int state) => (T)(object)(state - _offset);

        private Task<int> ExecuteGamezoneAsync(string methodName, string resultPropertyName, Dictionary<string, string>? methodParams = null) =>
            ExecuteAsync("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);

        private async Task<int> ExecuteAsync(string queryString,
            string methodName,
            string resultPropertyName,
            Dictionary<string, string>? methodParams = null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Executing WMI query... [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

            var mos = new ManagementObjectSearcher("ROOT\\WMI", queryString);
            var managementObjects = await mos.GetAsync().ConfigureAwait(false);
            var managementObject = managementObjects.FirstOrDefault();

            if (managementObject is null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"No results in query [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

                throw new InvalidOperationException("No results in query");
            }

            var mo = (ManagementObject)managementObject;
            var methodParamsObject = mo.GetMethodParameters(methodName);
            if (methodParams is not null)
                foreach (var pair in methodParams)
                    methodParamsObject[pair.Key] = pair.Value;

            var result = mo.InvokeMethod(methodName, methodParamsObject, null)?.Properties[resultPropertyName].Value;
            var intResult = Convert.ToInt32(result);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Executed WMI query with result {intResult} [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

            return intResult;
        }
    }
}