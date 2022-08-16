using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ACAdapterDisconnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, IPowerAutomationPipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is disconnected";

        public async Task<bool> IsSatisfiedAsync(object? context) => await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false) == PowerAdapterStatus.Disconnected;

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterDisconnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterDisconnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
