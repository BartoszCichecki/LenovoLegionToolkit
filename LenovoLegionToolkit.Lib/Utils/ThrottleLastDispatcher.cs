using System;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThrottleLastDispatcher(TimeSpan interval, string? tag = null)
{
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task DispatchAsync(Func<Task> task)
    {
        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            var token = _cancellationTokenSource.Token;

            await Task.Delay(interval, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            if (tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Allowing... [tag={tag}]");

            await task().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Throttling... [tag={tag}]");
        }
    }
}
