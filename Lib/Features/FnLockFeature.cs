using System;

namespace LenovoLegionToolkit.Lib.Features
{
    public enum FnLockState
    {
        Off,
        On
    }

    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        public FnLockFeature() : base(DriverProvider.EnergyDriver, 0x831020E8)
        {
        }

        protected override byte GetInternalStatus()
        {
            return 0x2;
        }

        protected override byte[] ToInternal(FnLockState state)
        {
            switch (state)
            {
                case FnLockState.Off:
                    return new byte[] { 0xF };
                case FnLockState.On:
                    return new byte[] { 0xE };
                default:
                    throw new Exception("Invalid state");
            }
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