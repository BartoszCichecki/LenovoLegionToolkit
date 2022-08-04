using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class BatteryFeature : AbstractDriverFeature<BatteryState>
    {
        public BatteryFeature() : base(Drivers.GetEnergy, 0x831020F8) { }

        protected override uint GetInBufferValue() => 0xFF;

        protected override Task<uint[]> ToInternalAsync(BatteryState state)
        {
            uint[] result;
            switch (state)
            {
                case BatteryState.Conservation:
                    if (LastState == BatteryState.RapidCharge)
                        result = new uint[] { 0x8, 0x3 };
                    else
                        result = new uint[] { 0x3 };
                    break;
                case BatteryState.Normal:
                    if (LastState == BatteryState.Conservation)
                        result = new uint[] { 0x5 };
                    else
                        result = new uint[] { 0x8 };
                    break;
                case BatteryState.RapidCharge:
                    if (LastState == BatteryState.Conservation)
                        result = new uint[] { 0x5, 0x7 };
                    else
                        result = new uint[] { 0x7 };
                    break;
                default:
                    throw new InvalidOperationException("Invalid state.");
            }
            return Task.FromResult(result);
        }

        protected override Task<BatteryState> FromInternalAsync(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 17)) // is charging?
            {
                if (GetNthBit(state, 26))
                    return Task.FromResult(BatteryState.RapidCharge);
                else
                    return Task.FromResult(BatteryState.Normal);
            }

            if (GetNthBit(state, 29))
                return Task.FromResult(BatteryState.Conservation);

            throw new InvalidOperationException($"Unknown battery state: {state}");
        }
    }
}