using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using Microsoft.Win32.SafeHandles;
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
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var handle = Devices.GetRGBKeyboard();
            if (handle is null)
                throw new InvalidOperationException("RGB Keyboard unsupported.");

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

                var handle = Devices.GetRGBKeyboard();
                if (handle is null)
                    throw new InvalidOperationException("RGB Keyboard unsupported.");

                var index = state.ActivePresetIndex;
                var preset = state.Presets[index];
                var str = Convert(preset);

                await SendHidReport(handle!, str).ConfigureAwait(false);
            }
        }

        public async Task SetNextPresetAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var handle = Devices.GetRGBKeyboard();
            if (handle is null)
                throw new InvalidOperationException("RGB Keyboard unsupported.");

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                var state = _settings.Store.State;

                var index = state.ActivePresetIndex;
                var presets = state.Presets;

                var newIndex = (index + 1) % 3;

                _settings.Store.State = new(newIndex, presets);
                _settings.SynchronizeStore();

                var preset = state.Presets[newIndex];
                var str = Convert(preset);

                await SendHidReport(handle!, str).ConfigureAwait(false);
            }
        }


        private static async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await Vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == VantageStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }

        private Task SendHidReport(SafeFileHandle handle, RGBKeyboardStateEx str) => Task.Run(() =>
        {
            var size = Marshal.SizeOf<RGBKeyboardStateEx>();
            var bytes = new byte[size];

            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(str, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            if (!Native.HidD_SetFeature(handle, ref bytes, (uint)bytes.Length))
                NativeUtils.ThrowIfWin32Error("HidD_SetFeature");
        });

        public RGBKeyboardStateEx Convert(RGBKeyboardBacklightPreset preset)
        {
            var result = new RGBKeyboardStateEx
            {
                Header = new byte[] { 0xCC, 0x16 },
                Unused = new byte[13],
                Padding = 0x0
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
