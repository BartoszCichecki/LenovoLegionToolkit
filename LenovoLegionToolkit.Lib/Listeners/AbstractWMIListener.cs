using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public abstract class AbstractWMIListener<TEventArgs, TValue, TRawValue> : IListener<TEventArgs> where TEventArgs : EventArgs
{
    private readonly Func<Action<TRawValue>, IDisposable> _listen;

    private IDisposable? _disposable;

    public event EventHandler<TEventArgs>? Changed;

    protected AbstractWMIListener(Func<Action<TRawValue>, IDisposable> listen)
    {
        _listen = listen;
    }

    public Task StartAsync()
    {
        try
        {
            if (_disposable is not null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Already started. [listener={GetType().Name}]");
                return Task.CompletedTask;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [listener={GetType().Name}]");

            _disposable = _listen(Handler);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't start listener. [listener={GetType().Name}]", ex);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping... [listener={GetType().Name}]");

            _disposable?.Dispose();
            _disposable = null;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't stop listener. [listener={GetType().Name}]", ex);
        }

        return Task.CompletedTask;
    }

    protected abstract TValue GetValue(TRawValue value);

    protected abstract TEventArgs GetEventArgs(TValue value);

    protected abstract Task OnChangedAsync(TValue value);

    protected void RaiseChanged(TValue value) => Changed?.Invoke(this, GetEventArgs(value));

    private async void Handler(TRawValue properties)
    {
        try
        {
            var value = GetValue(properties);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [value={value}, listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to handle event.  [listener={GetType().Name}]", ex);
        }
    }
}
