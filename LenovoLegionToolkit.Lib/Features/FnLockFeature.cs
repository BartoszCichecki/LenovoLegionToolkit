using System;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        public FnLockFeature() : base(Drivers.GetEnergy, 0x831020E8) { }

        protected override byte GetInternalStatus() => 0x2;

        protected override byte[] ToInternal(FnLockState state)
        {
            return state switch
            {
                FnLockState.Off => new byte[] { 0xF },
                FnLockState.On => new byte[] { 0xE },
                _ => throw new Exception("Invalid state"),
            };
        }

        protected override FnLockState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 18))
                return FnLockState.On;
            return FnLockState.Off;
        }
    }
}