using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
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

    public class ProcessesAreRunning : IAutomationPipelineTrigger
    {
        public string DisplayName => "When app is running";

        public string[] ProcessNames { get; }

        public ProcessesAreRunning(string[] processNames) => ProcessNames = processNames;

        public Task<bool> IsSatisfiedAsync() => Task.Run(() =>
        {
            return ProcessNames.SelectMany(pn => Process.GetProcessesByName(pn)).Any();
        });

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunning(ProcessNames);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesAreRunning running &&
                   Enumerable.SequenceEqual(ProcessNames, running.ProcessNames);
        }

        public override int GetHashCode() => HashCode.Combine(ProcessNames);
    }

    public class ProcessesAreNotRunning : IAutomationPipelineTrigger
    {
        public string DisplayName => "When app is not running";

        public string[] ProcessNames { get; }

        public ProcessesAreNotRunning(string[] processNames) => ProcessNames = processNames;

        public Task<bool> IsSatisfiedAsync() => Task.Run(() =>
        {
            return ProcessNames.SelectMany(pn => Process.GetProcessesByName(pn)).IsEmpty();
        });

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreNotRunning(ProcessNames);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesAreNotRunning running &&
                   Enumerable.SequenceEqual(ProcessNames, running.ProcessNames);
        }

        public override int GetHashCode() => HashCode.Combine(ProcessNames);
    }
}
