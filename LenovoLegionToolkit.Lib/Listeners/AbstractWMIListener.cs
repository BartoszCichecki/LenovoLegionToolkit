using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

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
        }

        public AbstractWMIListener(string scope, string eventName)
        {
            _scope = scope;
            _query = $"SELECT * FROM {eventName}";
        }

        public void Start()
        {
            try
            {
                if (_disposable is not null)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Already started. [listener={GetType().Name}]");
                    return;
                }

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

        public void Stop()
        {
            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Stopping... [listener={GetType().Name}]");

                _disposable?.Dispose();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't stop listener. [listener={GetType().Name}]", ex);
            }
        }

        protected abstract T GetValue(PropertyDataCollection properties);

        protected abstract Task OnChangedAsync(T value);

        private async void Handler(PropertyDataCollection properties)
        {
            var value = GetValue(properties);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [value={value}, listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            Changed?.Invoke(this, value);
        }
    }
}
