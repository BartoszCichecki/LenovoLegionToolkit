using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class WhiteKeyboardBacklightFeature : AbstractDriverFeature<WhiteKeyboardBacklightStatus>
    {
        public WhiteKeyboardBacklightFeature() : base(Drivers.GetEnergy, 0x83102144) { }

        public async override Task<WhiteKeyboardBacklightStatus> GetStateAsync()
        {
            await IsSupportedAsync().ConfigureAwait(false);
            return await base.GetStateAsync().ConfigureAwait(false);
        }

        public async override Task SetStateAsync(WhiteKeyboardBacklightStatus state)
        {
            await IsSupportedAsync().ConfigureAwait(false);
            await base.SetStateAsync(state).ConfigureAwait(false);
        }

        protected override uint GetInternalStatus() => 0x22;

        protected override uint[] ToInternal(WhiteKeyboardBacklightStatus state)
        {
            return state switch
            {
                WhiteKeyboardBacklightStatus.Off => new uint[] { 0x00023 },
                WhiteKeyboardBacklightStatus.Low => new uint[] { 0x10023 },
                WhiteKeyboardBacklightStatus.High => new uint[] { 0x20023 },
                _ => throw new Exception("Invalid state"),
            };
        }

        protected override WhiteKeyboardBacklightStatus FromInternal(uint state)
        {
            return state switch
            {
                0x1 => WhiteKeyboardBacklightStatus.Off,
                0x3 => WhiteKeyboardBacklightStatus.Low,
                0x5 => WhiteKeyboardBacklightStatus.High,
                _ => throw new Exception("Invalid state"),
            };
        }

        private async Task IsSupportedAsync()
        {
            var (_, outBuffer) = await SendCodeAsync(DriverHandle(), ControlCode, 0x1).ConfigureAwait(false);
            outBuffer >>= 1;
            if (outBuffer != 0x2)
                throw new InvalidOperationException("Not supported.");
        }
    }
}
