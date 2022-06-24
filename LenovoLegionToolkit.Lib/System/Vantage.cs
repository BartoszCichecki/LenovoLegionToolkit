namespace LenovoLegionToolkit.Lib.System
{
    public class Vantage : SoftwareDisabler
    {
        protected override string[] ScheduledTasksPaths => new[]
        {
            "Lenovo\\BatteryGauge",
            "Lenovo\\ImController",
            "Lenovo\\ImController\\Plugins",
            "Lenovo\\ImController\\TimeBasedEvents",
            "Lenovo\\Vantage",
            "Lenovo\\Vantage\\Schedule",
        };

        protected override string[] ServiceNames => new[]
        {
            "ImControllerService",
            "LenovoVantageService",
        };
    }
}
