namespace LenovoLegionToolkit.Lib.Automation
{
    public interface IAutomationEvent { }

    public struct StartupAutomationEvent : IAutomationEvent { }

    public struct PowerStateAutomationEvent : IAutomationEvent { }

    public struct PowerModeAutomationEvent : IAutomationEvent
    {
        public PowerModeState PowerModeState { get; init; }
    }

    public struct ProcessAutomationEvent : IAutomationEvent
    {
        public ProcessEventInfo ProcessEventInfo { get; init; }
    }

    public struct TimeAutomationEvent : IAutomationEvent
    {
        public Time Time { get; init; }
    }
}
