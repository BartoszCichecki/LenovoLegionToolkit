using System;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThrottleFirstDispatcher
{
    private readonly AsyncLock _lock = new();

    private readonly TimeSpan _interval;
    private readonly string? _tag;

    private DateTime _lastEvent = DateTime.MinValue;

    public ThrottleFirstDispatcher(TimeSpan interval, string? tag = null)
    {
        _interval = interval;
        _tag = tag;
    }

    public async Task DispatchAsync(Func<Task> task)
    {
        using (await _lock.LockAsync())
        {
            var diff = DateTime.UtcNow - _lastEvent;

            if (diff < _interval)
            {
                if (_tag is not null && Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Throttling... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

                return;
            }

            if (_tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Allowing... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

            await task();

            _lastEvent = DateTime.UtcNow;
        }
    }
}
