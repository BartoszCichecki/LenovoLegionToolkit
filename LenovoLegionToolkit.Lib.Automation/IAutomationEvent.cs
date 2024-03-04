using System;

namespace LenovoLegionToolkit.Lib.Automation;

public interface IAutomationEvent { }

public readonly struct NativeWindowsMessageEvent : IAutomationEvent
{
    public NativeWindowsMessage Message { get; init; }
}

public struct StartupAutomationEvent : IAutomationEvent { }

public readonly struct PowerStateAutomationEvent : IAutomationEvent
{
    public PowerStateEvent Event { get; init; }
    public bool PowerAdapterStateChanged { get; init; }
}

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
    public bool Running { get; init; }
}

public readonly struct ProcessAutomationEvent : IAutomationEvent
{
    public ProcessEventInfoType Type { get; init; }

    public ProcessInfo ProcessInfo { get; init; }
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
