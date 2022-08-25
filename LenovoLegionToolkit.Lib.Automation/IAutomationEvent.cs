namespace LenovoLegionToolkit.Lib.Automation
{
    public interface IAutomationEvent { }

    public struct StartupAutomationEvent : IAutomationEvent { }

    public struct PowerAutomationEvent : IAutomationEvent { }

    public struct ProcessAutomationEvent : IAutomationEvent
    {
        public ProcessEventInfo ProcessEventInfo { get; init; }
    }
}
