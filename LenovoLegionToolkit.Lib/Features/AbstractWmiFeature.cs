using System;
using System.Collections.Generic;
using System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public abstract class AbstractWmiFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly string _methodNameSuffix;
        private readonly int _offset;
        private readonly string _supportMethodName;
        private readonly int _supportOffset;

        protected AbstractWmiFeature(string methodNameSuffix, int offset, string supportMethodName = null, int supportOffset = 0)
        {
            _methodNameSuffix = methodNameSuffix;
            _offset = offset;
            _supportMethodName = supportMethodName;
            _supportOffset = supportOffset;
        }

        public virtual T GetState()
        {
            Log.Instance.Trace($"Getting state... [feature={GetType().Name}]");
            if (!IsSupported())
            {
                Log.Instance.Trace($"Feature {_methodNameSuffix} is not supported [feature={GetType().Name}]");
                throw new NotSupportedException($"Feature {_methodNameSuffix} is not supported.");
            }

            var result = FromInternal(ExecuteGamezone("Get" + _methodNameSuffix, "Data"));
            Log.Instance.Trace($"State is {result} [feature={GetType().Name}]");
            return result;
        }

        public virtual void SetState(T state)
        {
            Log.Instance.Trace($"Setting state to {state}... [feature={GetType().Name}]");
            ExecuteGamezone("Set" + _methodNameSuffix, "Data",
                new Dictionary<string, string>
                {
                    {"Data", ToInternal(state).ToString()}
                });
            Log.Instance.Trace($"Set state to {state} [feature={GetType().Name}]");
        }

        private bool IsSupported()
        {
            if (_supportMethodName == null)
                return true;

            var value = ExecuteGamezone(_supportMethodName, "Data");
            return value > _supportOffset;
        }

        private int ToInternal(T state) => (int)(object)state + _offset;

        private T FromInternal(int state) => (T)(object)(state - _offset);

        private int ExecuteGamezone(string methodName, string resultPropertyName, Dictionary<string, string> methodParams = null)
        {
            return Execute("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);
        }

        private int Execute(string queryString,
            string methodName,
            string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            Log.Instance.Trace($"Executing WMI query... [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

            using var enumerator = new ManagementObjectSearcher("ROOT\\WMI", queryString).Get().GetEnumerator();
            if (!enumerator.MoveNext())
            {
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

            Log.Instance.Trace($"Executed WMI query with result {intResult} [feature={GetType().Name}, queryString={queryString}, methodName={methodName}, resultPropertyName={resultPropertyName}, methodParams.Count={methodParams?.Count}]");

            return intResult;
        }
    }
}