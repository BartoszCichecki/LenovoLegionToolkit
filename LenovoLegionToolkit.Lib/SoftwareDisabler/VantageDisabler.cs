namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class VantageDisabler : AbstractSoftwareDisabler
{
    protected override string[] ScheduledTasksPaths => new[]
    {
        "Lenovo\\BatteryGauge",
        "Lenovo\\ImController",
        "Lenovo\\ImController\\Plugins",
        "Lenovo\\ImController\\TimeBasedEvents",
        "Lenovo\\UDC",
        "Lenovo\\Vantage",
        "Lenovo\\Vantage\\Schedule"
    };

    protected override string[] ServiceNames => new[]
    {
        "ImControllerService",
        "LenovoVantageService"
    };

    protected override string[] ProcessNames => new[]
    {
        "LenovoVantage",
        "Lenovo.Modern.ImController"
    };
}
