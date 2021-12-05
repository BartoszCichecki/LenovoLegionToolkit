using System;

namespace LenovoLegionToolkit.Lib
{
    public struct RefreshRate
    {
        public static bool operator ==(RefreshRate left, RefreshRate right) => left.Equals(right);

        public static bool operator !=(RefreshRate left, RefreshRate right) => !(left == right);

        public int Frequency { get; }

        public RefreshRate(int frequency)
        {
            Frequency = frequency;
        }

        public override string ToString() => $"{Frequency} Hz";

        public override int GetHashCode() => HashCode.Combine(Frequency);

        public override bool Equals(object obj) => obj is RefreshRate rate && Frequency == rate.Frequency;
    }
}
