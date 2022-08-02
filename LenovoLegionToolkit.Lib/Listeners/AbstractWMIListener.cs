using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.Lib.Listeners
{
    public abstract class AbstractWMIListener<T> : IListener<T> where T : struct
    {
        private readonly string _scope;
        private readonly FormattableString _query;

        private IDisposable? _disposable;

        public event EventHandler<T>? Changed;

        public AbstractWMIListener(string scope, FormattableString query)
        {
            _scope = scope;
            _query = query;

            Start();
        }

        public AbstractWMIListener(string scope, string eventName)
        {
            _scope = scope;
            _query = $"SELECT * FROM {eventName}";

            Start();
        }

        private void Start()
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Starting... [listener={GetType().Name}]");

                _disposable = WMI.Listen(_scope, _query, Handler);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't start listener. [listener={GetType().Name}]", ex);
            }
        }

        protected abstract T GetValue(PropertyDataCollection properties);

        protected abstract Task OnChangedAsync(T value);

        private async void Handler(PropertyDataCollection properties)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [listener={GetType().Name}]");

            var value = GetValue(properties);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Value {value} [listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            Changed?.Invoke(this, value);
        }
    }
}
