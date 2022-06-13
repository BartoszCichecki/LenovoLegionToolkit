using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline
{
    public interface IAutomationPipelineTrigger
    {
        [JsonIgnore]
        string DisplayName { get; }

        Task<bool> IsSatisfiedAsync();

        IAutomationPipelineTrigger DeepCopy();
    }

    public class ACAdapterConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is connected";

        public Task<bool> IsSatisfiedAsync() => Task.FromResult(Power.IsPowerAdapterConnected());

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterConnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterConnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }

    public class ACAdapterDisconnectedAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is disconnected";

        public Task<bool> IsSatisfiedAsync() => Task.FromResult(!Power.IsPowerAdapterConnected());

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterDisconnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterDisconnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
