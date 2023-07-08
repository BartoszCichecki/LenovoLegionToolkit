using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public abstract class AbstractWMIListener<T1, T2> : IListener<T1> where T1 : struct
{
    private readonly Func<Action<T2>, IDisposable> _listen;

    private IDisposable? _disposable;

    public event EventHandler<T1>? Changed;

    protected AbstractWMIListener(Func<Action<T2>, IDisposable> listen)
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

    protected abstract T1 GetValue(T2 value);

    protected abstract Task OnChangedAsync(T1 value);

    protected void RaiseChanged(T1 value) => Changed?.Invoke(this, value);

    private async void Handler(T2 properties)
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
