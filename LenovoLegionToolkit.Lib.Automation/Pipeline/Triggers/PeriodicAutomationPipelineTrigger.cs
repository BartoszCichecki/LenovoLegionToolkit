using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class PeriodicAutomationPipelineTrigger : IPeriodicAutomationPipelineTrigger
    {
        public string DisplayName => Resource.PeriodicActionPipelineTrigger_DisplayName;

        public TimeSpan Period { get; }

        [JsonConstructor]
        public PeriodicAutomationPipelineTrigger(TimeSpan period)
        {
            Period = period;
        }

        public IAutomationPipelineTrigger DeepCopy() => new PeriodicAutomationPipelineTrigger(Period);
        public IPeriodicAutomationPipelineTrigger DeepCopy(TimeSpan Period) => new PeriodicAutomationPipelineTrigger(Period);

        public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
        {
            if (automationEvent is not TimeAutomationEvent)
                return Task.FromResult(false);

            return IsMatching();
        }

        public Task<bool> IsMatchingState()
        {
            return IsMatching();
        }

        private Task<bool> IsMatching()
        {
            var currentDayMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            var isPeriod = currentDayMinutes % Period.TotalMinutes == 0;

            if (isPeriod)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public void UpdateEnvironment(ref AutomationEnvironment environment) { }
    }
}
