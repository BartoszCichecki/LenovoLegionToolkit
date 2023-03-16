using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class TimeAutomationPipelineTrigger : ITimeAutomationPipelineTrigger
{
    public bool IsSunrise { get; }

    public bool IsSunset { get; }

    public Time? Time { get; }

    public string DisplayName => Resource.TimeAutomationPipelineTrigger_DisplayName;

    private readonly SunriseSunset _sunriseSunset = IoCContainer.Resolve<SunriseSunset>();

    [JsonConstructor]
    public TimeAutomationPipelineTrigger(bool isSunrise, bool isSunset, Time? time)
    {
        IsSunrise = isSunrise;
        IsSunset = isSunset;
        Time = time;
    }

    public async Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not TimeAutomationEvent e)
            return false;

        if (Time == e.Time)
            return true;

        var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

        if (IsSunrise && sunrise == e.Time)
            return true;

        if (IsSunset && sunset == e.Time)
            return true;

        return false;
    }

    public async Task<bool> IsMatchingState()
    {
        var now = DateTime.UtcNow;
        var time = new Time { Hour = now.Hour, Minute = now.Minute };

        if (Time == time)
            return true;

        var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

        if (IsSunrise && sunrise == time)
            return true;

        if (IsSunset && sunset == time)
            return true;

        return false;
    }

    public IAutomationPipelineTrigger DeepCopy() => new TimeAutomationPipelineTrigger(IsSunrise, IsSunset, Time);

    public ITimeAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time) => new TimeAutomationPipelineTrigger(isSunrise, isSunset, time);

    public override bool Equals(object? obj)
    {
        return obj is TimeAutomationPipelineTrigger t &&
               IsSunrise == t.IsSunrise &&
               IsSunset == t.IsSunset &&
               Time == t.Time;
    }

    public override int GetHashCode() => HashCode.Combine(IsSunrise, IsSunset, Time);

    public override string ToString() => $"{nameof(IsSunrise)}: {IsSunrise}, {nameof(IsSunset)}: {IsSunset}, {nameof(Time)}: {Time}";
}