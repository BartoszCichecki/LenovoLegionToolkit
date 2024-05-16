using System;
using System.ComponentModel;
using LenovoLegionToolkit.Lib.Macro.Utils.TypeConverters;

namespace LenovoLegionToolkit.Lib.Macro;

public readonly struct MacroEvent
{
    public MacroSource Source { get; init; }
    public MacroDirection Direction { get; init; }
    public ulong Key { get; init; }
    public TimeSpan Delay { get; init; }

    public bool IsUndefined() => Source == MacroSource.Unknown || Direction == MacroDirection.Unknown || Key < 1;

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
    public bool IgnoreDelays { get; init; }
    public int RepeatCount { get; init; }
    public MacroEvent[]? Events { get; init; }
}
