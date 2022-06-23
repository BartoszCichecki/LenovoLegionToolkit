using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public interface IAutomationPipelineTrigger
    {
        [JsonIgnore]
        string DisplayName { get; }

        bool IsSatisfied(object? context);

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
