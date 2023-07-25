using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public abstract class AbstractAutoListener<T> : IAutoListener<T>
{
    private readonly AsyncLock _startStopLock = new();

    private bool _started;

    private event EventHandler<T>? ChangedInternal;

    public event EventHandler<T>? Changed
    {
        add { ChangedInternal += value; StartStop(); }
        remove { ChangedInternal -= value; StartStop(); }
    }

    private void StartStop()
    {
        Task.Run(async () =>
        {
            using (await _startStopLock.LockAsync())
            {
                if (ChangedInternal?.GetInvocationList().Any() ?? false)
                    await StartInternalAsync().ConfigureAwait(false);
                else
                    await StopInternalAsync().ConfigureAwait(false);
            }
        });
    }

    private async Task StartInternalAsync()
    {
        if (_started)
            return;

        await StartAsync().ConfigureAwait(false);

        _started = true;
    }

    private async Task StopInternalAsync()
    {
        if (!_started)
            return;

        await StopAsync().ConfigureAwait(false);

        _started = false;
    }

    protected abstract Task StartAsync();

    protected abstract Task StopAsync();

    protected void RaiseChanged(T value) => ChangedInternal?.Invoke(this, value);
}
