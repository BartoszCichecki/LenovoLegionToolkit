using System;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class AlwaysOnUSBFeature : AbstractDriverFeature<AlwaysOnUSBState>
    {
        public AlwaysOnUSBFeature() : base(Drivers.GetEnergy, 0x831020E8) { }

        protected override byte GetInternalStatus() => 0x2;

        protected override byte[] ToInternal(AlwaysOnUSBState state)
        {
            return state switch
            {
                AlwaysOnUSBState.Off => new byte[] { 0xB, 0x12 },
                AlwaysOnUSBState.OnWhenSleeping => new byte[] { 0xA, 0x12 },
                AlwaysOnUSBState.OnAlways => new byte[] { 0xA, 0x13 },
                _ => throw new InvalidOperationException("Invalid state"),
            };
        }

        protected override AlwaysOnUSBState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 31)) // is on?
            {
                if (GetNthBit(state, 23))
                    return AlwaysOnUSBState.OnAlways;
                return AlwaysOnUSBState.OnWhenSleeping;
            }

            return AlwaysOnUSBState.Off;
        }
    }
}