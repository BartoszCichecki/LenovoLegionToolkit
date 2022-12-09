using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class DisplayChangeAutomationPipelineTrigger : IAutomationPipelineTrigger, IDisplayChangeAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => Resource.DisplayChangeAutomationPipelineTrigger_DisplayName;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            return Task.FromResult(automationEvent is DisplayChangedAutomationEvent);
        }

        public IAutomationPipelineTrigger DeepCopy() => new DisplayChangeAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is DisplayChangeAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
