using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public interface IAutomationPipelineTrigger
    {
        [JsonIgnore]
        string DisplayName { get; }

        Task<bool> IsSatisfiedAsync(object? context);

        IAutomationPipelineTrigger DeepCopy();
    }

    public interface IDisallowDuplicatesAutomationPipelineTrigger { }

    public interface IPowerAutomationPipelineTrigger { }

    public interface IProcessesAutomationPipelineTrigger
    {
        ProcessInfo[] Processes { get; }

        IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes);
    }
}
