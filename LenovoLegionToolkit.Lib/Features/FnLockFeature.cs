using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        public FnLockFeature() : base(Drivers.GetEnergy, 0x831020E8) { }

        protected override uint GetInBufferValue() => 0x2;

        protected override async Task<uint[]> ToInternalAsync(FnLockState state)
        {
            var lockOn = state switch
            {
                FnLockState.On => true,
                FnLockState.Off => false,
                _ => throw new InvalidOperationException("Invalid state"),
            };

            if (await ShouldFlipAsync().ConfigureAwait(false))
                lockOn = !lockOn;

            return lockOn ? new uint[] { 0xE } : new uint[] { 0xF };
        }

        protected override async Task<FnLockState> FromInternalAsync(uint state)
        {
            state = state.ReverseEndianness();

            var lockOn = false;
            if (state.GetNthBit(18))
                lockOn = true;
            if (await ShouldFlipAsync().ConfigureAwait(false))
                lockOn = !lockOn;

            return lockOn ? FnLockState.On : FnLockState.Off;
        }

        private async Task<bool> ShouldFlipAsync()
        {
            var mi = await Compatibility.GetMachineInformation().ConfigureAwait(false);
            return mi.Properties.ShouldFlipFnLock;
        }
    }
}