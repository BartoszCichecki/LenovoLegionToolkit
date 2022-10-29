using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class ThrottleFirstDispatcher
    {
        private readonly object _lock = new();

        private readonly TimeSpan _interval;

        private DateTime _lastEvent = DateTime.MinValue;

        public ThrottleFirstDispatcher(TimeSpan interval)
        {
            _interval = interval;
        }

        public Task DispatchAsync(Func<Task> task)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var diff = now - _lastEvent;
                _lastEvent = now;

                if (diff < _interval)
                    return Task.CompletedTask;
            }

            return task();
        }
    }
}
