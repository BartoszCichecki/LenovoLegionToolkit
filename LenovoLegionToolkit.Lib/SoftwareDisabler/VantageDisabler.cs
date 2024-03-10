using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class VantageDisabler : AbstractSoftwareDisabler
{
    protected override IEnumerable<string> ScheduledTasksPaths =>
    [
        "Lenovo\\BatteryGauge",
        "Lenovo\\ImController",
        "Lenovo\\ImController\\Plugins",
        "Lenovo\\ImController\\TimeBasedEvents",
        "Lenovo\\UDC",
        "Lenovo\\Vantage",
        "Lenovo\\Vantage\\Schedule"
    ];

    protected override IEnumerable<string> ServiceNames =>
    [
        "ImControllerService",
        "LenovoVantageService"
    ];

    protected override IEnumerable<string> ProcessNames =>
    [
        "LenovoVantage",
        "Lenovo.Modern.ImController"
    ];
}
