#define MOCK_RGB

using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using NeoSmart.AsyncLock;

#if !MOCK_RGB
using System.Runtime.InteropServices;
#endif

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        private readonly AsyncLock _ioLock = new();

        private readonly RGBKeyboardSettings _settings;

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings) => _settings = settings;

        public bool IsSupported()
        {
#if MOCK_RGB
            return true;
#else
            return Devices.GetRGBKeyboard() is not null;
#endif
        }

        public async Task<RGBKeyboardBacklightState> GetStateAsync()
        {
#if !MOCK_RGB
            var handle = Devices.GetRGBKeyboard();
            if (handle is null)
                throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

            await ThrowIfVantageEnabled().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return _settings.Store.State;
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _settings.Store.State = state;
                _settings.SynchronizeStore();

                var selectedPreset = state.SelectedPreset;

                RGBKeyboardStateEx str;
                if (selectedPreset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[selectedPreset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task SetPresetAsync(RGBKeyboardBacklightPreset preset)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                var state = _settings.Store.State;
                var presets = state.Presets;

                _settings.Store.State = new(preset, presets);
                _settings.SynchronizeStore();

                RGBKeyboardStateEx str;
                if (preset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[preset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task SetNextPresetAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                var state = _settings.Store.State;

                var newPreset = state.SelectedPreset.Next();
                var presets = state.Presets;

                _settings.Store.State = new(newPreset, presets);
                _settings.SynchronizeStore();

                RGBKeyboardStateEx str;
                if (newPreset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[newPreset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        private static async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await Vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == VantageStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }

        private Task SendToDevice(RGBKeyboardStateEx str) => Task.Run(() =>
        {
#if !MOCK_RGB
            var handle = Devices.GetRGBKeyboard();
            if (handle is null)
                throw new InvalidOperationException("RGB Keyboard unsupported.");


            var ptr = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf<RGBKeyboardStateEx>();
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(str, ptr, true);

                if (!Native.HidD_SetFeature(handle, ptr, (uint)size))
                    NativeUtils.ThrowIfWin32Error("HidD_SetFeature");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
#endif
        });

        public RGBKeyboardStateEx CreateOffState()
        {
            return new()
            {
                Header = new byte[] { 0xCC, 0x16 },
                Unused = new byte[13],
                Padding = 0x0,
                Effect = 0x1,
                Brightness = 0x1,
                Zone1Rgb = new byte[3],
                Zone2Rgb = new byte[3],
                Zone3Rgb = new byte[3],
                Zone4Rgb = new byte[3],
            };
        }

        public RGBKeyboardStateEx Convert(RGBKeyboardBacklightSettings preset)
        {
            var result = new RGBKeyboardStateEx
            {
                Header = new byte[] { 0xCC, 0x16 },
                Unused = new byte[13],
                Padding = 0x0,
                Zone1Rgb = new byte[] { 0xFF, 0xFF, 0xFF },
                Zone2Rgb = new byte[] { 0xFF, 0xFF, 0xFF },
                Zone3Rgb = new byte[] { 0xFF, 0xFF, 0xFF },
                Zone4Rgb = new byte[] { 0xFF, 0xFF, 0xFF },
            };

            switch (preset.Effect)
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

            switch (preset.Brightness)
            {
                case RGBKeyboardBrightness.Low:
                    result.Brightness = 1;
                    break;
                case RGBKeyboardBrightness.High:
                    result.Brightness = 2;
                    break;
            }

            if (preset.Effect != RGBKeyboardEffect.Static)
            {
                switch (preset.Speed)
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

            if (preset.Effect == RGBKeyboardEffect.Static || preset.Effect == RGBKeyboardEffect.Breath)
            {
                result.Zone1Rgb = new[] { preset.Zone1.R, preset.Zone1.G, preset.Zone1.B };
                result.Zone2Rgb = new[] { preset.Zone2.R, preset.Zone2.G, preset.Zone2.B };
                result.Zone3Rgb = new[] { preset.Zone3.R, preset.Zone3.G, preset.Zone3.B };
                result.Zone4Rgb = new[] { preset.Zone4.R, preset.Zone4.G, preset.Zone4.B };
            }

            return result;
        }
    }
}
