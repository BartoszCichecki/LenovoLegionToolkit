using System;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThrottleFirstDispatcher(TimeSpan interval, string? tag = null)
{
    private readonly AsyncLock _lock = new();

    private DateTime _lastEvent = DateTime.MinValue;

    public async Task DispatchAsync(Func<Task> task)
    {
        using (await _lock.LockAsync().ConfigureAwait(false))
        {
            var diff = DateTime.UtcNow - _lastEvent;

            if (diff < interval)
            {
                if (tag is not null && Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Throttling... [tag={tag}, diff={diff.TotalMilliseconds}ms]");

                return;
            }

            if (tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Allowing... [tag={tag}, diff={diff.TotalMilliseconds}ms]");

            await task().ConfigureAwait(false);

            _lastEvent = DateTime.UtcNow;
        }
    }
}
