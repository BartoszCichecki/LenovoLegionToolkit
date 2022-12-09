namespace LenovoLegionToolkit.Lib.Automation;

public interface IAutomationEvent { }

public struct StartupAutomationEvent : IAutomationEvent { }
public struct ExternalDisplayConnectedAutomationEvent : IAutomationEvent { }
public struct ExternalDisplayDisconnectedAutomationEvent : IAutomationEvent { }

public struct PowerStateAutomationEvent : IAutomationEvent { }

public readonly struct PowerModeAutomationEvent : IAutomationEvent
{
    public PowerModeState PowerModeState { get; init; }
}

public readonly struct ProcessAutomationEvent : IAutomationEvent
{
    public ProcessEventInfo ProcessEventInfo { get; init; }
}

public readonly struct TimeAutomationEvent : IAutomationEvent
{
    public Time Time { get; init; }
}