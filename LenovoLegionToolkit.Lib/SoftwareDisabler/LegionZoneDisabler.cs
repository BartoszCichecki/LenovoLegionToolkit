using System;

namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class LegionZoneDisabler : AbstractSoftwareDisabler
{
    protected override string[] ScheduledTasksPaths => Array.Empty<string>();
    protected override string[] ServiceNames => new[] { "LZService" };
    protected override string[] ProcessNames => new[] { "LegionZone", "LZTray" };
}
