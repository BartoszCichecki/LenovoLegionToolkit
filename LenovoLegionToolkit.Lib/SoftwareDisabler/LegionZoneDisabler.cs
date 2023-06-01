using System;
using System.Collections.Generic;

namespace LenovoLegionToolkit.Lib.SoftwareDisabler;

public class LegionZoneDisabler : AbstractSoftwareDisabler
{
    protected override IEnumerable<string> ScheduledTasksPaths => Array.Empty<string>();
    protected override IEnumerable<string> ServiceNames => new[] { "LZService" };
    protected override IEnumerable<string> ProcessNames => new[] { "LegionZone", "LZTray" };
}
