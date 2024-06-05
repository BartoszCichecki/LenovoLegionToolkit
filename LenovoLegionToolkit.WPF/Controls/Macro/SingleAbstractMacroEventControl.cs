using System;
using System.Collections.Generic;
using LenovoLegionToolkit.Lib.Macro;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public class SingleAbstractMacroEventControl : AbstractMacroEventControl
{
    private MacroEvent _macroEvent;

    public override IEnumerable<MacroEvent> GetEvents() => [_macroEvent];

    protected override TimeSpan TotalDelay => _macroEvent.Delay;

    public override void Set(MacroEvent macroEvent)
    {
        _macroEvent = macroEvent;

        base.Set(macroEvent);
    }
}
