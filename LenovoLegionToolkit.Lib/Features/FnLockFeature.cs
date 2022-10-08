using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        public FnLockFeature() : base(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_SETTINGS) { }

        protected override uint GetInBufferValue() => 0x2;

        protected override Task<uint[]> ToInternalAsync(FnLockState state)
        {
            var lockOn = state switch
            {
                FnLockState.On => true,
                FnLockState.Off => false,
                _ => throw new InvalidOperationException("Invalid state"),
            };

            var value = lockOn ? new uint[] { 0xE } : new uint[] { 0xF };
            return Task.FromResult(value);
        }

        protected override Task<FnLockState> FromInternalAsync(uint state)
        {
            var value = state.GetNthBit(10) ? FnLockState.On : FnLockState.Off;
            return Task.FromResult(value);
        }
    }
}