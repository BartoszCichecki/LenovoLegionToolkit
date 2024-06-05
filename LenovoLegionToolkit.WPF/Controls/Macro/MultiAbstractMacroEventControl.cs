using System;
using System.Collections.Generic;
using System.Linq;
using LenovoLegionToolkit.Lib.Macro;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public class MultiAbstractMacroEventControl : AbstractMacroEventControl
{
    private readonly List<MacroEvent> _macroEvents = [];

    public override IEnumerable<MacroEvent> GetEvents() => _macroEvents;

    protected override TimeSpan TotalDelay => _macroEvents
        .Select(me => me.Delay)
        .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

    public override void Set(MacroEvent macroEvent)
    {
        _macroEvents.Add(macroEvent);

        base.Set(macroEvent);
    }
}
