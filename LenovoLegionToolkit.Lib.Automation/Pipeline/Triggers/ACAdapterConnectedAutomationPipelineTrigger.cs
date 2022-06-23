using System;
using LenovoLegionToolkit.Lib.System;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class ACAdapterConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger, IPowerAutomationPipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is connected";

        public bool IsSatisfied(object? context) => Power.IsPowerAdapterConnected();

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterConnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterConnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
