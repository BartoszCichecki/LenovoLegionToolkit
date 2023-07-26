using System;
using System.Threading.Tasks;
using System.Timers;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class TimeAutoListener : AbstractAutoListener<(Time, DayOfWeek)>
{
    private readonly Timer _timer;

    public TimeAutoListener()
    {
        _timer = new Timer(60_000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.AutoReset = true;
    }

    protected override Task StartAsync()
    {
        if (!_timer.Enabled)
            _timer.Enabled = true;

        return Task.CompletedTask;
    }

    protected override Task StopAsync()
    {
        _timer.Enabled = false;

        return Task.CompletedTask;
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e) => RaiseChanged((TimeExtensions.UtcNow, DateTime.UtcNow.DayOfWeek));
}
