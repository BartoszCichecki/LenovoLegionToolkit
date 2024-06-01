using System.Windows.Input;
using Humanizer;
using Humanizer.Localisation;
using LenovoLegionToolkit.Lib.Macro;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Macro;

public partial class MacroEventControl
{
    public MacroEvent MacroEvent { get; private set; }

    public MacroEventControl()
    {
        InitializeComponent();
    }

    public void Set(MacroEvent macroEvent)
    {
        MacroEvent = macroEvent;

        _card.Icon = macroEvent.Direction switch
        {
            MacroDirection.Up => SymbolRegular.ArrowCircleUp24,
            MacroDirection.Down => SymbolRegular.ArrowCircleDown24,
            _ => SymbolRegular.Empty
        };

        _header.Title = (macroEvent.Source, macroEvent.Key) switch
        {
            (MacroSource.Keyboard, _) => KeyInterop.KeyFromVirtualKey((int)macroEvent.Key).ToString(),
            (MacroSource.Mouse, >= 0xFF) => "XBUTTON" + (macroEvent.Key >> 16),
            (MacroSource.Mouse, 1) => "LBUTTON",
            (MacroSource.Mouse, 2) => "RBUTTON",
            (MacroSource.Mouse, 3) => "MBUTTON",
            (MacroSource.Mouse, _) => "BUTTON" + macroEvent.Key,
            _ => string.Empty
        };

        _header.Subtitle = macroEvent.Delay.Humanize(maxUnit: TimeUnit.Millisecond);
    }
}
