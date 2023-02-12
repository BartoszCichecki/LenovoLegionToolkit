using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public interface IAutomationPipelineTrigger
{
    [JsonIgnore]
    string DisplayName { get; }

    Task<bool> IsMatchingEvent(IAutomationEvent automationEvent);

    Task<bool> IsMatchingState();

    IAutomationPipelineTrigger DeepCopy();
}

public interface IDisallowDuplicatesAutomationPipelineTrigger { }

public interface ICompositeAutomationPipelineTrigger
{
    public IAutomationPipelineTrigger[] Triggers { get; }
}

public interface INativeWindowsMessagePipelineTrigger { }

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