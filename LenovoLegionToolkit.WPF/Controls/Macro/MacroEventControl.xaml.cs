﻿using System.Windows.Input;
using Humanizer;
using Humanizer.Localisation;
using LenovoLegionToolkit.Lib.Extensions;
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
            MacroDirection.Wheel => SymbolRegular.ArrowRotateClockwise24,
            MacroDirection.HorizontalWheel => SymbolRegular.ArrowRotateClockwise24,
            MacroDirection.Move => SymbolRegular.ArrowMove24,
            _ => SymbolRegular.Empty
        };

        _header.Title = (macroEvent.Source, macroEvent.Direction, macroEvent.Key) switch
        {
            (MacroSource.Keyboard, _, _) => KeyInterop.KeyFromVirtualKey((int)macroEvent.Key).ToString(),
            (MacroSource.Mouse, MacroDirection.Move, _) => "MOVE",
            (MacroSource.Mouse, MacroDirection.Wheel, >= 0x80000000) => "WHEEL DOWN",
            (MacroSource.Mouse, MacroDirection.Wheel, _) => "WHEEL UP",
            (MacroSource.Mouse, MacroDirection.HorizontalWheel, >= 0x80000000) => "WHEEL LEFT",
            (MacroSource.Mouse, MacroDirection.HorizontalWheel, _) => "WHEEL RIGHT",
            (MacroSource.Mouse, _, >= 0xFF) => "XBUTTON" + (macroEvent.Key >> 16),
            (MacroSource.Mouse, _, 1) => "LBUTTON",
            (MacroSource.Mouse, _, 2) => "RBUTTON",
            (MacroSource.Mouse, _, 3) => "MBUTTON",
            (MacroSource.Mouse, _, _) => "BUTTON" + macroEvent.Key,
            _ => string.Empty
        };

        _header.Subtitle = macroEvent.Source.GetDisplayName();

        if (macroEvent.Direction is not MacroDirection.Move)
            _header.Subtitle += $" • {macroEvent.Delay.Humanize(maxUnit: TimeUnit.Millisecond)}";
    }
}
