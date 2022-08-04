using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        private bool? _shouldFlip;

        public FnLockFeature() : base(Drivers.GetEnergy, 0x831020E8) { }

        protected override uint GetInBufferValue() => 0x2;

        protected override async Task<uint[]> ToInternalAsync(FnLockState state)
        {
            var lockOn = state switch
            {
                FnLockState.On => true,
                FnLockState.Off => false,
                _ => throw new Exception("Invalid state"),
            };

            if (await ShouldFlipAsync().ConfigureAwait(false))
                lockOn = !lockOn;

            return lockOn ? new uint[] { 0xE } : new uint[] { 0xF };
        }

        protected override async Task<FnLockState> FromInternalAsync(uint state)
        {
            state = ReverseEndianness(state);

            var lockOn = false;
            if (GetNthBit(state, 18))
                lockOn = true;
            if (await ShouldFlipAsync().ConfigureAwait(false))
                lockOn = !lockOn;

            return lockOn ? FnLockState.On : FnLockState.Off;
        }

        private async Task<bool> ShouldFlipAsync()
        {
            if (!_shouldFlip.HasValue)
            {
                var (_, outBuffer) = await SendCodeAsync(DriverHandle(), ControlCode, GetInBufferValue()).ConfigureAwait(false);
                outBuffer = ReverseEndianness(outBuffer);
                _shouldFlip = GetNthBit(outBuffer, 19);
            }

            return _shouldFlip.Value;
        }
    }
}