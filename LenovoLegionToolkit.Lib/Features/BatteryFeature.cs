using System;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class BatteryFeature : AbstractDriverFeature<BatteryState>
    {
        public BatteryFeature() : base(Drivers.GetEnergy, 0x831020F8) { }

        protected override uint GetInternalStatus() => 0xFF;

        protected override uint[] ToInternal(BatteryState state)
        {
            switch (state)
            {
                case BatteryState.Conservation:
                    if (LastState == BatteryState.RapidCharge)
                        return new uint[] { 0x8, 0x3 };
                    else
                        return new uint[] { 0x3 };
                case BatteryState.Normal:
                    if (LastState == BatteryState.Conservation)
                        return new uint[] { 0x5 };
                    else
                        return new uint[] { 0x8 };
                case BatteryState.RapidCharge:
                    if (LastState == BatteryState.Conservation)
                        return new uint[] { 0x5, 0x7 };
                    else
                        return new uint[] { 0x7 };
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

            throw new InvalidOperationException($"Unknown battery state: {state}");
        }
    }
}