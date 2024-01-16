﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public interface IAutomationPipelineTrigger
{
    [JsonIgnore]
    string DisplayName { get; }

    Task<bool> IsMatchingEvent(IAutomationEvent automationEvent);

    Task<bool> IsMatchingState();

    void UpdateEnvironment(ref AutomationEnvironment environment);

    IAutomationPipelineTrigger DeepCopy();
}

public interface IDisallowDuplicatesAutomationPipelineTrigger : IAutomationPipelineTrigger { }

public interface ICompositeAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    public IAutomationPipelineTrigger[] Triggers { get; }
}

public interface INativeWindowsMessagePipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IOnStartupAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IPowerStateAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IPowerModeAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    PowerModeState PowerModeState { get; }

    IPowerModeAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState);
}

public interface IGodModePresetChangedAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    Guid PresetId { get; }

    IGodModePresetChangedAutomationPipelineTrigger DeepCopy(Guid powerModeState);
}

public interface IGameAutomationPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }

public interface IProcessesAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    ProcessInfo[] Processes { get; }

    IProcessesAutomationPipelineTrigger DeepCopy(ProcessInfo[] processes);
}

public interface ITimeAutomationPipelineTrigger : IAutomationPipelineTrigger
{
    bool IsSunrise { get; }
    bool IsSunset { get; }
    Time? Time { get; }
    DayOfWeek[] Days { get; }

    ITimeAutomationPipelineTrigger DeepCopy(bool isSunrise, bool isSunset, Time? time, DayOfWeek[] day);
}

public interface IUserInactivityPipelineTrigger : IAutomationPipelineTrigger
{
    TimeSpan InactivityTimeSpan { get; }

    IUserInactivityPipelineTrigger DeepCopy(TimeSpan timeSpan);
}

public interface IWiFiConnectedPipelineTrigger : IAutomationPipelineTrigger
{
    string? Ssid { get; }

    IWiFiConnectedPipelineTrigger DeepCopy(string? ssid);
}

public interface IWiFiDisconnectedPipelineTrigger : IDisallowDuplicatesAutomationPipelineTrigger { }
