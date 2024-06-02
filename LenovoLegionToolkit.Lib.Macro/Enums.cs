using System;
using System.ComponentModel.DataAnnotations;
using LenovoLegionToolkit.Lib.Macro.Resources;

namespace LenovoLegionToolkit.Lib.Macro;

public enum MacroDirection
{
    Unknown,
    Down,
    Up,
    Wheel,
    HorizontalWheel,
    Move
}

[Flags]
public enum MacroRecorderSettings
{
    Keyboard = 1,
    Mouse = 2,
    Movement = 4
}

public enum MacroSource
{
    Unknown,
    [Display(ResourceType = typeof(Resource), Name = "MacroSource_Keyboard")]
    Keyboard,
    [Display(ResourceType = typeof(Resource), Name = "MacroSource_Mouse")]
    Mouse
}
