using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ACAdapterConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is connected";

        public Task<bool> IsSatisfiedAsync(object? context) => Task.FromResult(Power.IsPowerAdapterConnected());

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterConnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterConnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
