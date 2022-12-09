using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ExternalDisplayConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, IExternalDisplayConnectedAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => Resource.ExternalDisplayConnectedAutomationPipelineTrigger_DisplayName;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            return Task.FromResult(automationEvent is ExternalDisplayConnectedAutomationEvent);
        }

        public IAutomationPipelineTrigger DeepCopy() => new ExternalDisplayConnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ExternalDisplayConnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
