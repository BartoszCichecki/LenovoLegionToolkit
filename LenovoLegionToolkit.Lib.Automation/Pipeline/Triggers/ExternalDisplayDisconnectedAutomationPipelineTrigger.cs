using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ExternalDisplayDisconnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, IExternalDisplayDisconnectedAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => Resource.ExternalDisplayDisconnectedAutomationPipelineTrigger_DisplayName;

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            return Task.FromResult(automationEvent is ExternalDisplayDisconnectedAutomationEvent);
        }

        public IAutomationPipelineTrigger DeepCopy() => new ExternalDisplayDisconnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ExternalDisplayDisconnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
