﻿using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public interface IAutomationPipelineTrigger
{
    [JsonIgnore]
    string DisplayName { get; }

    Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent);

    IAutomationPipelineTrigger DeepCopy();
}

public interface IDisallowDuplicatesAutomationPipelineTrigger : IAutomationPipelineTrigger { }

public interface INativeWindowsMessagePipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IOnStartupAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IPowerStateAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IPowerModeAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    PowerModeState PowerModeState { get; }

    IAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState);
}

public interface IGameAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IProcessesAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    ProcessInfo[] Processes { get; }

    IAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes);
}

public interface ITimeAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    bool IsSunrise { get; }
    bool IsSunset { get; }
    Time? Time { get; }
    IAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time);
}