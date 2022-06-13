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

        Task<bool> IsSatisfiedAsync(object? context);

        IAutomationPipelineTrigger DeepCopy();
    }

    public class ACAdapterConnectedAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is connected";

        public Task<bool> IsSatisfiedAsync(object? context) => Task.FromResult(Power.IsPowerAdapterConnected());

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterConnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterConnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }

    public class ACAdapterDisconnectedAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "When AC adapter is disconnected";

        public Task<bool> IsSatisfiedAsync(object? context) => Task.FromResult(!Power.IsPowerAdapterConnected());

        public IAutomationPipelineTrigger DeepCopy() => new ACAdapterDisconnectedAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is ACAdapterDisconnectedAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }

    public class ProcessesAreRunningAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        public string DisplayName => "When app is running";

        public string[] ProcessNames { get; }

        public ProcessesAreRunningAutomationPipelineTrigger(string[] processNames) => ProcessNames = processNames;

        public Task<bool> IsSatisfiedAsync(object? context) => Task.Run(() => ProcessNames.SelectMany(Process.GetProcessesByName).Any());

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesAreRunningAutomationPipelineTrigger(ProcessNames);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesAreRunningAutomationPipelineTrigger running &&
                   Enumerable.SequenceEqual(ProcessNames, running.ProcessNames);
        }

        public override int GetHashCode() => HashCode.Combine(ProcessNames);
    }

    public class ProcessesStopRunningAutomationPipelineTrigger : IAutomationPipelineTrigger
    {
        public string DisplayName => "When app closes";

        public string[] ProcessNames { get; }

        public ProcessesStopRunningAutomationPipelineTrigger(string[] processNames) => ProcessNames = processNames;

        public async Task<bool> IsSatisfiedAsync(object? context)
        {
            if (context is not ProcessEventInfo pei || pei.Type != ProcessEventInfoType.Stopped)
                return false;

            var matches = ProcessNames.Contains(pei.Name, StringComparer.InvariantCultureIgnoreCase);
            if (!matches)
                return false;

            var result = await Task.Run(() => ProcessNames.SelectMany(pn => Process.GetProcessesByName(pn)).IsEmpty()).ConfigureAwait(false);
            return result;
        }

        public IAutomationPipelineTrigger DeepCopy() => new ProcessesStopRunningAutomationPipelineTrigger(ProcessNames);

        public override bool Equals(object? obj)
        {
            return obj is ProcessesStopRunningAutomationPipelineTrigger running &&
                   Enumerable.SequenceEqual(ProcessNames, running.ProcessNames);
        }

        public override int GetHashCode() => HashCode.Combine(ProcessNames);
    }
}
