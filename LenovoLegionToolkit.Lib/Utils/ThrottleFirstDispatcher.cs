using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThrottleFirstDispatcher
{
    private readonly object _lock = new();

    private readonly TimeSpan _interval;
    private readonly string? _tag;

    private DateTime _lastEvent = DateTime.MinValue;

    public ThrottleFirstDispatcher(TimeSpan interval, string? tag = null)
    {
        _interval = interval;
        _tag = tag;
    }

    public Task DispatchAsync(Func<Task> task)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var diff = now - _lastEvent;
            _lastEvent = now;

            if (diff < _interval)
            {
                if (_tag is not null && Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Throttling... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

                return Task.CompletedTask;
            }

            if (_tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Allowing... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

            return task();
        }
    }

    public void Dispatch(Action task)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var diff = now - _lastEvent;
            _lastEvent = now;

            if (diff < _interval)
            {
                if (_tag is not null && Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Throttling... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

                return;
            }

            if (_tag is not null && Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Allowing... [tag={_tag}, diff={diff.TotalMilliseconds}ms]");

            task();
        }
    }
}