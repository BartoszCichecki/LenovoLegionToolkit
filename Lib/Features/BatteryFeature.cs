using System;

namespace LenovoLegionToolkit.Lib.Features
{
    public enum BatteryState
    {
        Conservation,
        Normal,
        RapidCharge
    }

    public class BatteryFeature : AbstractDriverFeature<BatteryState>
    {
        public BatteryFeature() : base(DriverProvider.EnergyDriver, 0x831020F8)
        {
        }

        protected override byte GetInternalStatus()
        {
            return 0xFF;
        }

        protected override byte[] ToInternal(BatteryState state)
        {
            switch (state)
            {
                case BatteryState.Conservation:
                    if (LastState == BatteryState.RapidCharge)
                        return new byte[] { 0x8, 0x3 };
                    else
                        return new byte[] { 0x3 };
                case BatteryState.Normal:
                    if (LastState == BatteryState.Conservation)
                        return new byte[] { 0x5 };
                    else
                        return new byte[] { 0x8 };
                case BatteryState.RapidCharge:
                    if (LastState == BatteryState.Conservation)
                        return new byte[] { 0x5, 0x7 };
                    else
                        return new byte[] { 0x7 };
                default:
                    throw new Exception("Invalid state");
            }
        }

        protected override BatteryState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 17)) // is charging?
            {
                if (GetNthBit(state, 26))
                {
                    return BatteryState.RapidCharge;
                }

                return BatteryState.Normal;
            }

            if (GetNthBit(state, 29))
                return BatteryState.Conservation;

            throw new Exception("Unknown battery state: " + state);
        }
    }
}