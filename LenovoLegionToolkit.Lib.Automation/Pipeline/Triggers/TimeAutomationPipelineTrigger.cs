using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class TimeAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        public bool IsSunrise { get; }

        public bool IsSunset { get; }

        public Time? Time { get; }

        public string DisplayName => "At specified time";

        private readonly SunriseSunset _sunriseSunset = IoCContainer.Resolve<SunriseSunset>();

        [JsonConstructor]
        public TimeAutomationPipelineTrigger(bool isSunrise, bool isSunset, Time? time)
        {
            IsSunrise = isSunrise;
            IsSunset = isSunset;
            Time = time;
        }

        public async Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is StartupAutomationEvent)
                return false;

            if (automationEvent is not TimeAutomationEvent timeAutomationEvent)
                return false;

            if (Time == timeAutomationEvent.Time)
                return true;

            var (sunrise, sunset) = await _sunriseSunset.GetSunriseSunsetAsync().ConfigureAwait(false);

            if (IsSunrise && sunrise == timeAutomationEvent.Time)
                return true;

            if (IsSunset && sunset == timeAutomationEvent.Time)
                return true;

            return false;
        }

        public IAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time) => new TimeAutomationPipelineTrigger(isSunrise, isSunset, time);

        public IAutomationPipelineTrigger DeepCopy() => new TimeAutomationPipelineTrigger(IsSunrise, IsSunset, Time);

        public override bool Equals(object? obj)
        {
            return obj is TimeAutomationPipelineTrigger trigger &&
                   IsSunrise == trigger.IsSunrise &&
                   IsSunset == trigger.IsSunset &&
                   Time == trigger.Time;
        }

        public override int GetHashCode() => HashCode.Combine(IsSunrise, IsSunset, Time);
    }
}
