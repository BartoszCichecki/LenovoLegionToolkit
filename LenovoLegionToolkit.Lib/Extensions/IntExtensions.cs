namespace LenovoLegionToolkit.Lib.Extensions;

public static class IntExtensions
{
    public static bool IsBitSet(this int value, int position) => (value & (1 << position)) != 0;
}
