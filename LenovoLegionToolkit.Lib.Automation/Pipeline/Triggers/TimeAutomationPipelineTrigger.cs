using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class TimeAutomationPipelineTrigger : ITimeAutomationPipelineTrigger
{
    public bool IsSunrise { get; }

    public bool IsSunset { get; }

    public Time? Time { get; }

    public DayOfWeek[] Days { get; }

    public string DisplayName => Resource.TimeAutomationPipelineTrigger_DisplayName;

    private readonly SunriseSunset _sunriseSunset = IoCContainer.Resolve<SunriseSunset>();

    [JsonConstructor]
    public TimeAutomationPipelineTrigger(bool isSunrise, bool isSunset, Time? time, DayOfWeek[]? days)
    {
        IsSunrise = isSunrise;
        IsSunset = isSunset;
        Time = time;
        Days = days ?? Array.Empty<DayOfWeek>();
    }

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not TimeAutomationEvent e)
            return false;

        return await IsMatching(e.Time, e.Day).ConfigureAwait(false);
    }

    public async Task<bool> IsMatchingState()
    {
        var now = DateTime.UtcNow;
        var time = new Time { Hour = now.Hour, Minute = now.Minute };
        var day = now.DayOfWeek;

        return await IsMatching(time, day).ConfigureAwait(false);
    }

    public void UpdateEnvironment(AutomationEnvironment environment)
    {
        environment.IsSunset = IsSunset;
        environment.IsSunrise = IsSunrise;
        environment.Time = Time;
        environment.Days = Days;
    }

    private async Task<bool> IsMatching(Time time, DayOfWeek dayOfWeek)
    {
        if (Time == time && (Days.IsEmpty() || Days.Contains(dayOfWeek)))
            return true;

        var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

        if (IsSunrise && sunrise == time)
            return true;

        if (IsSunset && sunset == time)
            return true;

        return false;
    }
    public IAutomationPipelineTrigger DeepCopy() => new TimeAutomationPipelineTrigger(IsSunrise, IsSunset, Time, Days);

    public ITimeAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time, DayOfWeek[] days) => new TimeAutomationPipelineTrigger(isSunrise, isSunset, time, days);

    public override bool Equals(object? obj)
    {
        return obj is TimeAutomationPipelineTrigger t &&
               IsSunrise == t.IsSunrise &&
               IsSunset == t.IsSunset &&
               Time == t.Time &&
               Days == t.Days;
    }

    public override int GetHashCode() => HashCode.Combine(IsSunrise, IsSunset, Time, Days);

    public override string ToString() =>
        $"{nameof(IsSunrise)}: {IsSunrise}," +
        $" {nameof(IsSunset)}: {IsSunset}," +
        $" {nameof(Time)}: {Time}," +
        $" {nameof(Days)}: {Days}";
}
