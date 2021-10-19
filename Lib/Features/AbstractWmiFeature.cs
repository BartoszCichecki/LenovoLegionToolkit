using System;
using System.Collections.Generic;
using System.Management;

namespace LenovoLegionToolkit.Lib.Features
{
    public class AbstractWmiFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly string _methodNameSuffix;
        private readonly int _offset;

        protected AbstractWmiFeature(string methodNameSuffix, int offset)
        {
            _methodNameSuffix = methodNameSuffix;
            _offset = offset;
        }

        public T GetState()
        {
            var internalValue = ExecuteGamezone("Get" + _methodNameSuffix, "Data");
            return FromInternal(internalValue);
        }

        public void SetState(T state)
        {
            ExecuteGamezone("Set" + _methodNameSuffix, "Data",
                new Dictionary<string, string>
                {
                    {"Data", ToInternal(state).ToString()}
                });
        }

        private int ToInternal(T state)
        {
            return (int)(object)state + _offset;
        }

        private T FromInternal(int state)
        {
            return (T)(object)(state - _offset);
        }

        private static int ExecuteGamezone(string methodName, string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            return Execute("SELECT * FROM LENOVO_GAMEZONE_DATA", methodName, resultPropertyName, methodParams);
        }

        private static int Execute(string queryString, string methodName, string resultPropertyName,
            Dictionary<string, string> methodParams = null)
        {
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var objectQuery = new ObjectQuery(queryString);
            using (var enumerator = new ManagementObjectSearcher(scope, objectQuery).Get().GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception("No results in query");
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
}