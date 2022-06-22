using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        private readonly AsyncLock _ioLock = new();

        private readonly RGBKeyboardSettings _settings;

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings) => _settings = settings;

        public async Task<RGBKeyboardBacklightState> GetStateAsync()
        {
            await CheckVantageStatus().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return _settings.Store.State;
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state)
        {
            await CheckVantageStatus().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _settings.Store.State = state;
                _settings.SynchronizeStore();
            }
        }

        private static async Task CheckVantageStatus()
        {
            var vantageStatus = await Vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == VantageStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }

        public RGBKeyboardStateEx Convert(RGBKeyboardBacklightPreset state)
        {
            var result = new RGBKeyboardStateEx
            {
                Header = new byte[] { 0xCC, 0x16 },
                Unused = new byte[13],
                Padding = 0x0
            };

            switch (state.Effect)
            {
                case RGBKeyboardEffect.Static:
                    result.Effect = 1;
                    break;
                case RGBKeyboardEffect.Breath:
                    result.Effect = 3;
                    break;
                case RGBKeyboardEffect.WaveRTL:
                    result.Effect = 4;
                    result.WaveRTL = 1;
                    break;
                case RGBKeyboardEffect.WaveLTR:
                    result.Effect = 4;
                    result.WaveLTR = 1;
                    break;
                case RGBKeyboardEffect.Smooth:
                    result.Effect = 6;
                    break;
            }

            if (state.Effect != RGBKeyboardEffect.Static)
            {
                switch (state.Speed)
                {
                    case RBGKeyboardSpeed.Slowest:
                        result.Speed = 1;
                        break;
                    case RBGKeyboardSpeed.Slow:
                        result.Speed = 2;
                        break;
                    case RBGKeyboardSpeed.Fast:
                        result.Speed = 3;
                        break;
                    case RBGKeyboardSpeed.Fastest:
                        result.Speed = 4;
                        break;
                }
            }

            if (state.Effect == RGBKeyboardEffect.Static || state.Effect == RGBKeyboardEffect.Breath)
            {
                result.Zone1Rgb = new[] { state.Zone1.R, state.Zone1.G, state.Zone1.B };
                result.Zone2Rgb = new[] { state.Zone2.R, state.Zone2.G, state.Zone2.B };
                result.Zone3Rgb = new[] { state.Zone3.R, state.Zone3.G, state.Zone3.B };
                result.Zone4Rgb = new[] { state.Zone4.R, state.Zone4.G, state.Zone4.B };
            }

            return result;
        }
    }
}
