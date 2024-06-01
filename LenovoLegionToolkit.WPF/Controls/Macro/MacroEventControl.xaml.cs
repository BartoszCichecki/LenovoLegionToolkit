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
        _header.Title = KeyInterop.KeyFromVirtualKey((int)macroEvent.Key).ToString();
        _header.Subtitle = macroEvent.Delay.Humanize(maxUnit: TimeUnit.Millisecond);
    }
}
