using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public interface IAutomationPipelineTrigger
{
    [JsonIgnore]
    string DisplayName { get; }

    Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent);

    IAutomationPipelineTrigger DeepCopy();
}

public interface IDisplayChangeAutomationPipelineTrigger { }

public interface IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IOnStartupAutomationPipelineTrigger { }

public interface IPowerStateAutomationPipelineTrigger { }

public interface IPowerModeAutomationPipelineTrigger
{
    PowerModeState PowerModeState { get; }

    IAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState);
}

public interface IProcessesAutomationPipelineTrigger
{
    ProcessInfo[] Processes { get; }

    IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes);
}

public interface ITimeAutomationPipelineTrigger
{
    bool IsSunrise { get; }
    bool IsSunset { get; }
    Time? Time { get; }
    IAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time);
}