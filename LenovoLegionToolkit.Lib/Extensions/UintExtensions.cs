using System;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class UintExtensions
    {
        public static uint ReverseEndianness(this uint state)
        {
            var bytes = BitConverter.GetBytes(state);
            Array.Reverse(bytes, 0, bytes.Length);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static bool GetNthBit(this uint num, int n) => (num & (1 << n)) != 0;

        public static uint SetNthBit(this uint num, int n, bool state) => state ? num | (1U << n) : num & ~(1U << n);
    }
}
