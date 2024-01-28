using System;
using System.Threading.Tasks;
using AsyncKeyedLock;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public abstract class AbstractAutoListener<T> : IAutoListener<T>
{
    private readonly AsyncNonKeyedLocker _startStopLock = new();

    private bool _started;

    private event EventHandler<T>? Changed;

    public async Task SubscribeChangedAsync(EventHandler<T> eventHandler)
    {
        Changed += eventHandler;
        await StartStopAsync().ConfigureAwait(false);
    }

    public async Task UnsubscribeChangedAsync(EventHandler<T> eventHandler)
    {
        Changed -= eventHandler;
        await StartStopAsync().ConfigureAwait(false);
    }

    private async Task StartStopAsync()
    {
        using (await _startStopLock.LockAsync().ConfigureAwait(false))
        {
            var subscribers = Changed?.GetInvocationList().Length ?? 0;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Subscribers: {subscribers}. [type={GetType().Name}]");

            if (subscribers > 0)
                await StartInternalAsync().ConfigureAwait(false);
            else
                await StopInternalAsync().ConfigureAwait(false);
        }
    }

    private async Task StartInternalAsync()
    {
        if (_started)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Already started. [type={GetType().Name}]");

            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting... [type={GetType().Name}]");

        await StartAsync().ConfigureAwait(false);

        _started = true;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Started. [type={GetType().Name}]");
    }

    private async Task StopInternalAsync()
    {
        if (!_started)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Already stopped. [type={GetType().Name}]");

            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping... [type={GetType().Name}]");

        await StopAsync().ConfigureAwait(false);

        _started = false;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopped. [type={GetType().Name}]");
    }

    protected abstract Task StartAsync();

    protected abstract Task StopAsync();

    protected void RaiseChanged(T value) => Changed?.Invoke(this, value);
}
