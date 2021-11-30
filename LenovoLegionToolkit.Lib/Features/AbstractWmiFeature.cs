using System;
using System.Collections.Generic;
using System.Management;

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
            if (!IsSupported())
                throw new NotSupportedException($"Feature {_methodNameSuffix} is not supported.");

            return FromInternal(ExecuteGamezone("Get" + _methodNameSuffix, "Data"));
        }

        public virtual void SetState(T state)
        {
            ExecuteGamezone("Set" + _methodNameSuffix, "Data",
                new Dictionary<string, string>
                {
                    {"Data", ToInternal(state).ToString()}
                });
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

        private static int ExecuteGamezone(string methodName, string resultPropertyName, Dictionary<string, string> methodParams = null)
        {
            return Execute("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);
        }

        private static int Execute(string queryString, string methodName, string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var objectQuery = new ObjectQuery(queryString);
            using var enumerator = new ManagementObjectSearcher(scope, objectQuery).Get().GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("No results in query");
            var mo = (ManagementObject)enumerator.Current;
            var methodParamsObject = mo.GetMethodParameters(methodName);
            if (methodParams != null)
                foreach (var pair in methodParams)
                    methodParamsObject[pair.Key] = pair.Value;

            var result = mo.InvokeMethod(methodName, methodParamsObject, null)?.Properties[resultPropertyName].Value;
            return Convert.ToInt32(result);
        }
    }
}