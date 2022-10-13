using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ACAdapterDisconnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, IPowerStateAutomationPipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => Resource.ACAdapterDisconnectedAutomationPipelineTrigger_DisplayName;

        public async Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is not (PowerStateAutomationEvent or StartupAutomationEvent))
                return false;

            return await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false) == PowerAdapterStatus.Disconnected;
        }

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterDisconnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterDisconnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
