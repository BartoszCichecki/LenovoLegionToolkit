using System;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

#pragma warning disable IDE0052 // Remove unread private members

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

            Start();
        }

        private void Start()
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Starting... [listener={GetType().Name}]");

                _disposable = WMI.Listen("ROOT\\WMI", $"SELECT * FROM {_eventName}", Handler);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't start listener: {ex.Demystify()} [listener={GetType().Name}]");
            }
        }

        protected abstract Task OnChangedAsync(T value);

        private async void Handler(PropertyDataCollection properties)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [listener={GetType().Name}]");

            var property = properties[_property];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (T)(object)(propertyValue - _offset);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Value {value} [listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            Changed?.Invoke(this, value);
        }
    }
}
