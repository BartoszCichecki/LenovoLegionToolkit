using System;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThrottleLastDispatcher
{
    private readonly TimeSpan _interval;
    private readonly string? _tag;

    private CancellationTokenSource? _cancellationTokenSource;

    public ThrottleLastDispatcher(TimeSpan interval, string? tag = null)
    {
        _interval = interval;
        _tag = tag;
    }

    public async Task DispatchAsync(Func<Task> task)
    {
        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            var token = _cancellationTokenSource.Token;

            await Task.Delay(_interval, token)
                .ContinueWith(async t =>
                {
                    if (!t.IsCompletedSuccessfully)
                        return;

                    if (_tag is not null && Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Allowing... [tag={_tag}]");

                    await task();
                }, token);
        }
        catch (TaskCanceledException)
        {
            if (_tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Throttling... [tag={_tag}]");
        }
    }
}