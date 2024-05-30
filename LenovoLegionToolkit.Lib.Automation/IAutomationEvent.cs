using System;

namespace LenovoLegionToolkit.Lib.Automation;

public interface IAutomationEvent;

public readonly struct HDRAutomationEvent(bool? isHDROn) : IAutomationEvent
{
    public bool? IsHDROn { get; } = isHDROn;
}

public readonly struct NativeWindowsMessageEvent(NativeWindowsMessage message, object? data) : IAutomationEvent
{
    public NativeWindowsMessage Message { get; } = message;
    public object? Data { get; } = data;
}

public struct StartupAutomationEvent : IAutomationEvent;

public readonly struct PowerStateAutomationEvent(PowerStateEvent powerStateEvent, bool powerAdapterStateChanged)
    : IAutomationEvent
{
    public PowerStateEvent PowerStateEvent { get; } = powerStateEvent;
    public bool PowerAdapterStateChanged { get; } = powerAdapterStateChanged;
}

public readonly struct PowerModeAutomationEvent(PowerModeState powerModeState) : IAutomationEvent
{
    public PowerModeState PowerModeState { get; } = powerModeState;
}

public readonly struct CustomModePresetAutomationEvent(Guid id) : IAutomationEvent
{
    public Guid Id { get; } = id;
}

public readonly struct GameAutomationEvent(bool running) : IAutomationEvent
{
    public bool Running { get; } = running;
}

public readonly struct ProcessAutomationEvent(ProcessEventInfoType type, ProcessInfo processInfo) : IAutomationEvent
{
    public ProcessEventInfoType Type { get; } = type;

    public ProcessInfo ProcessInfo { get; } = processInfo;
}

public readonly struct TimeAutomationEvent(Time time, DayOfWeek day) : IAutomationEvent
{
    public Time Time { get; } = time;
    public DayOfWeek Day { get; } = day;
}

public readonly struct UserInactivityAutomationEvent(TimeSpan inactivityTimeSpan)
    : IAutomationEvent
{
    public TimeSpan InactivityTimeSpan { get; } = inactivityTimeSpan;
}

public readonly struct WiFiAutomationEvent(bool isConnected, string? ssid) : IAutomationEvent
{
    public bool IsConnected { get; } = isConnected;
    public string? Ssid { get; } = ssid;
}
