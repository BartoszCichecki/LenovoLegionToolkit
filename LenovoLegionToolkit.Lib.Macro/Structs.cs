using System;

namespace LenovoLegionToolkit.Lib.Macro;

public readonly struct MacroSequence
{
    public bool IgnoreDelays { get; init; }
    public int RepeatCount { get; init; }
    public MacroEvent[]? Events { get; init; }
}

public readonly struct MacroEvent
{
    public MacroDirection Direction { get; init; }
    public ulong Key { get; init; }
    public TimeSpan Delay { get; init; }

    public bool IsUndefined() => Direction == MacroDirection.Unknown || Key < 1;

    public override string ToString() => $"{nameof(Direction)}: {Direction}, {nameof(Key)}: {Key}, {nameof(Delay)}: {Delay}";
}
