using System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class AlwaysOnUsbFeature : AbstractDriverFeature<AlwaysOnUsbState>
    {
        public AlwaysOnUsbFeature() : base(Drivers.Energy, 0x831020E8) { }

        protected override byte GetInternalStatus() => 0x2;

        protected override byte[] ToInternal(AlwaysOnUsbState state)
        {
            return state switch
            {
                AlwaysOnUsbState.Off => new byte[] { 0xB, 0x12 },
                AlwaysOnUsbState.OnWhenSleeping => new byte[] { 0xA, 0x12 },
                AlwaysOnUsbState.OnAlways => new byte[] { 0xA, 0x13 },
                _ => throw new InvalidOperationException("Invalid state"),
            };
        }

        protected override AlwaysOnUsbState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 31)) // is on?
            {
                if (GetNthBit(state, 23))
                    return AlwaysOnUsbState.OnAlways;
                return AlwaysOnUsbState.OnWhenSleeping;
            }

            return AlwaysOnUsbState.Off;
        }
    }
}