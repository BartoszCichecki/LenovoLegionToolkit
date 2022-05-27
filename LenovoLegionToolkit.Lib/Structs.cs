using System;

namespace LenovoLegionToolkit.Lib
{
    public struct CPUBoostMode
    {
        public int Value { get; }
        public string Name { get; }

        public CPUBoostMode(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    public struct WindowSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public WindowSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }

    public struct RefreshRate : IDisplayName
    {
        public static bool operator ==(RefreshRate left, RefreshRate right) => left.Equals(right);

        public static bool operator !=(RefreshRate left, RefreshRate right) => !(left == right);

        public int Frequency { get; }

        public string DisplayName => ToString();

        public RefreshRate(int frequency)
        {
            Frequency = frequency;
        }

        public override string ToString() => $"{Frequency} Hz";

        public override int GetHashCode() => HashCode.Combine(Frequency);

        public override bool Equals(object? obj) => obj is RefreshRate rate && Frequency == rate.Frequency;
    }
}
