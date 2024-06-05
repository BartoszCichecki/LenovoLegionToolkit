using System;
using System.ComponentModel;
using System.Drawing;
using LenovoLegionToolkit.Lib.Macro.Utils.TypeConverters;

namespace LenovoLegionToolkit.Lib.Macro;

public readonly struct MacroEvent
{
    public MacroSource Source { get; init; }
    public MacroDirection Direction { get; init; }
    public uint Key { get; init; }
    public Point Point { get; init; }
    public TimeSpan Delay { get; init; }

    public bool IsUndefined() => Source == MacroSource.Unknown || Direction == MacroDirection.Unknown;

    public override string ToString() => $"{nameof(Source)}:{Source}, {nameof(Direction)}: {Direction}, {nameof(Key)}: {Key}, {nameof(Delay)}: {Delay}";
}

[TypeConverter(typeof(MacroIdentifierTypeConverter))]
public readonly struct MacroIdentifier(MacroSource source, ulong key)
{
    public MacroSource Source { get; } = source;

    public ulong Key { get; } = key;

    #region Equality

    public override bool Equals(object? obj) => obj is MacroIdentifier other && Source == other.Source && Key == other.Key;

    public override int GetHashCode() => HashCode.Combine((int)Source, Key);

    public static bool operator ==(MacroIdentifier left, MacroIdentifier right) => left.Equals(right);

    public static bool operator !=(MacroIdentifier left, MacroIdentifier right) => !(left == right);

    #endregion
}

public readonly struct MacroSequence
{
    public int RepeatCount { get; init; }
    public bool IgnoreDelays { get; init; }
    public bool InterruptOnOtherKey { get; init; }
    public MacroEvent[]? Events { get; init; }
}
