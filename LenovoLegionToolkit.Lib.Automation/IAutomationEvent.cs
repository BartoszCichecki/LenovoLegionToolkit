using System;

namespace LenovoLegionToolkit.Lib.Automation;

public interface IAutomationEvent { }

public readonly struct NativeWindowsMessageEvent : IAutomationEvent
{
    public NativeWindowsMessage Message { get; init; }
}

public struct StartupAutomationEvent : IAutomationEvent { }

public struct PowerStateAutomationEvent : IAutomationEvent { }

public readonly struct PowerModeAutomationEvent : IAutomationEvent
{
    public PowerModeState PowerModeState { get; init; }
}

public readonly struct CustomModePresetAutomationEvent : IAutomationEvent
{
    public Guid Id { get; init; }
}

public readonly struct GameAutomationEvent : IAutomationEvent
{
    public bool Started { get; init; }
}

public readonly struct ProcessAutomationEvent : IAutomationEvent
{
    public ProcessEventInfo ProcessEventInfo { get; init; }
}

public readonly struct TimeAutomationEvent : IAutomationEvent
{
    public Time Time { get; init; }
    public DayOfWeek Day { get; init; }
}

public readonly struct UserInactivityAutomationEvent : IAutomationEvent
{
    public TimeSpan InactivityTimeSpan { get; init; }
    public TimeSpan ResolutionTimeSpan { get; init; }
}

public readonly struct WiFiAutomationEvent : IAutomationEvent
{
    public bool IsConnected { get; init; }
    public string? Ssid { get; init; }
}
