using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class TimeAutomationPipelineTrigger : ITimeAutomationPipelineTrigger
{
    public bool IsSunrise { get; }

    public bool IsSunset { get; }

    public Time? Time { get; }
    
    public Day? Day { get; }

    public string DisplayName => Resource.TimeAutomationPipelineTrigger_DisplayName;

    private readonly SunriseSunset _sunriseSunset = IoCContainer.Resolve<SunriseSunset>();

    [JsonConstructor]
    public TimeAutomationPipelineTrigger(bool isSunrise, bool isSunset, Time? time, Day? day)
    {
        IsSunrise = isSunrise;
        IsSunset = isSunset;
        Time = time;
        Day = day;
    }

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not TimeAutomationEvent e)
            return false;


        if (e.CurrentDay == Day)
            return true;

        var time = new Time { Hour = e.CurrentDay.Hour, Minute = e.CurrentDay.Minute };

        if (Time == time)
            return true;

        var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

        if (IsSunrise && sunrise == time)
            return true;

        if (IsSunset && sunset == time)
            return true;

        return false;
    }

    public async Task<bool> IsMatchingState()
    {
        var now = DateTime.UtcNow;
        var day = new Day { Hour = now.Hour, Minute = now.Minute, DayOfWeek = now.DayOfWeek };

        if (Day == day)
            return true;

        var time = Lib.Time.FromDay(day);

        if (Time == time)
            return true;

        var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

        if (IsSunrise && sunrise == time)
            return true;

        if (IsSunset && sunset == time)
            return true;

        return false;
    }

    public IAutomationPipelineTrigger DeepCopy() => new TimeAutomationPipelineTrigger(IsSunrise, IsSunset, Time, Day);

    public ITimeAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time, Day? day) => new TimeAutomationPipelineTrigger(isSunrise, isSunset, time, day);

    public override bool Equals(object? obj)
    {
        return obj is TimeAutomationPipelineTrigger t &&
               IsSunrise == t.IsSunrise &&
               IsSunset == t.IsSunset &&
               Time == t.Time &&
               Day == t.Day;
    }

    public override int GetHashCode() => HashCode.Combine(IsSunrise, IsSunset, Time, Day);

    public override string ToString() => $"{nameof(IsSunrise)}: {IsSunrise}, {nameof(IsSunset)}: {IsSunset}, {nameof(Time)}: {Time}, {nameof(Day)}: {Day}";
}