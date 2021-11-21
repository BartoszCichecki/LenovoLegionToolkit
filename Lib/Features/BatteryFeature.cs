using LenovoLegionToolkit.Lib.Utils;
using System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class BatteryFeature : AbstractDriverFeature<BatteryState>
    {
        public BatteryFeature() : base(Drivers.Energy, 0x831020F8) { }

        protected override byte GetInternalStatus() => 0xFF;

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
                    throw new InvalidOperationException("Invalid state.");
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

            throw new InvalidOperationException($"Unknown battery state: {state}.");
        }
    }
}