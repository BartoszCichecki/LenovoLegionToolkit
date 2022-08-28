using System;
using System.Timers;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class TimeListener : IListener<Time>
    {
        public event EventHandler<Time>? Changed;

        private readonly Timer _timer;

        public TimeListener()
        {
            _timer = new Timer(60_000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
        }

        public void Start() => _timer.Enabled = true;

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;
            Changed?.Invoke(this, new() { Hour = now.Hour, Minute = now.Minute });
        }
    }
}
