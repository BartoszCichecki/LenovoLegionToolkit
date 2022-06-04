using System;
using System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public abstract class AbstractWMIListener<T> : IListener<T> where T : struct, Enum, IComparable
    {
        private readonly string _eventName;
        private readonly string _property;
        private readonly int _offset;

        private IDisposable? _disposable;

        public event EventHandler<T>? Changed;

        public AbstractWMIListener(string eventName, string property, int offset)
        {
            _eventName = eventName;
            _property = property;
            _offset = offset;
        }

        public void Start()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [listener={GetType().Name}]");

            _disposable = WMI.Listen("ROOT\\WMI", $"SELECT * FROM {_eventName}", Handler);
        }

        public void Stop()
        {
            _disposable?.Dispose();
            _disposable = null;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped [listener={GetType().Name}]");
        }

        protected abstract void OnChanged(T value);

        private void Handler(PropertyDataCollection properties)
        {
            var property = properties[_property];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (T)(object)(propertyValue - _offset);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Value {value} [listener={GetType().Name}]");

            OnChanged(value);
            Changed?.Invoke(this, value);
        }
    }
}
