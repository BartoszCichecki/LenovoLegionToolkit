// #define MOCK_RGB

using System;
using System.Threading.Tasks;
using Windows.Win32;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using NeoSmart.AsyncLock;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;

#if !MOCK_RGB
using System.Runtime.InteropServices;
#endif

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        private static readonly AsyncLock IoLock = new();

        private readonly RGBKeyboardSettings _settings;

        private readonly Vantage _vantage;

        private SafeFileHandle? _deviceHandle;

        private SafeFileHandle? DeviceHandle
        {
            get
            {
                if (ForceDisable)
                    return null;

                _deviceHandle ??= Devices.GetRGBKeyboard();
                return _deviceHandle;
            }
        }

        public bool ForceDisable { get; set; }

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings, Vantage vantage)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
        }

        public Task<bool> IsSupportedAsync()
        {
#if MOCK_RGB
            return Task.FromResult(true);
#else
            return Task.FromResult(DeviceHandle is not null);
#endif
        }

        public async Task SetLightControlOwnerAsync(bool enable, bool restorePreset = false)
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
                try
                {
#if !MOCK_RGB
                    var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                    await ThrowIfVantageEnabled().ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Taking ownership...");

#if !MOCK_RGB
                    await WMI.CallAsync("ROOT\\WMI",
                        $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                        "SetLightControlOwner",
                        new() { { "Data", enable ? 1 : 0 } }).ConfigureAwait(false);
#endif

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Ownership set to {enable}, restoring profile...");

                    if (restorePreset)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Restoring preset...");

                        await SetCurrentPresetAsync().ConfigureAwait(false);

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Restored preset");
                    }
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Can't take ownership.", ex);

                    throw;
                }
            }
        }

        public async Task<RGBKeyboardBacklightState> GetStateAsync()
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                return _settings.Store.State;
            }
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state)
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                _settings.Store.State = state;
                _settings.SynchronizeStore();

                var selectedPreset = state.SelectedPreset;

                LENOVO_RGB_KEYBOARD_STATE str;
                if (selectedPreset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[selectedPreset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task SetPresetAsync(RGBKeyboardBacklightPreset preset)
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                var state = _settings.Store.State;
                var presets = state.Presets;

                _settings.Store.State = new(preset, presets);
                _settings.SynchronizeStore();

                var str = preset == RGBKeyboardBacklightPreset.Off
                    ? CreateOffState()
                    : Convert(state.Presets[preset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task<RGBKeyboardBacklightPreset> SetNextPresetAsync()
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                var state = _settings.Store.State;

                var newPreset = state.SelectedPreset.Next();
                var presets = state.Presets;

                _settings.Store.State = new(newPreset, presets);
                _settings.SynchronizeStore();

                LENOVO_RGB_KEYBOARD_STATE str;
                if (newPreset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[newPreset]);

                await SendToDevice(str).ConfigureAwait(false);

                return newPreset;
            }
        }

        private async Task SetCurrentPresetAsync()
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                var state = _settings.Store.State;

                var preset = state.SelectedPreset;

                LENOVO_RGB_KEYBOARD_STATE str;
                if (preset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                    str = Convert(state.Presets[preset]);

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }

        private unsafe Task SendToDevice(LENOVO_RGB_KEYBOARD_STATE str) => Task.Run(() =>
        {
#if !MOCK_RGB
            var handle = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");

            var ptr = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf<LENOVO_RGB_KEYBOARD_STATE>();
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(str, ptr, false);

                if (!PInvoke.HidD_SetFeature(handle, ptr.ToPointer(), (uint)size))
                    PInvokeExtensions.ThrowIfWin32Error("HidD_SetFeature");
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
#endif
        });

        private LENOVO_RGB_KEYBOARD_STATE CreateOffState()
        {
            return new()
            {
                Header = new byte[] { 0xCC, 0x16 },
                Unused = new byte[13],
                Padding = 0,
                Effect = 0,
                WaveLTR = 0,
                WaveRTL = 0,
                Brightness = 0,
                Zone1Rgb = new byte[3],
                Zone2Rgb = new byte[3],
                Zone3Rgb = new byte[3],
                Zone4Rgb = new byte[3],
            };
        }

        private LENOVO_RGB_KEYBOARD_STATE Convert(RGBKeyboardBacklightBacklightPresetDescription preset)
        {
            var result = new LENOVO_RGB_KEYBOARD_STATE
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
                case RGBKeyboardBacklightEffect.Static:
                    result.Effect = 1;
                    break;
                case RGBKeyboardBacklightEffect.Breath:
                    result.Effect = 3;
                    break;
                case RGBKeyboardBacklightEffect.WaveRTL:
                    result.Effect = 4;
                    result.WaveRTL = 1;
                    break;
                case RGBKeyboardBacklightEffect.WaveLTR:
                    result.Effect = 4;
                    result.WaveLTR = 1;
                    break;
                case RGBKeyboardBacklightEffect.Smooth:
                    result.Effect = 6;
                    break;
            }

            switch (preset.Brightness)
            {
                case RGBKeyboardBacklightBrightness.Low:
                    result.Brightness = 1;
                    break;
                case RGBKeyboardBacklightBrightness.High:
                    result.Brightness = 2;
                    break;
            }

            if (preset.Effect != RGBKeyboardBacklightEffect.Static)
            {
                switch (preset.Speed)
                {
                    case RBGKeyboardBacklightSpeed.Slowest:
                        result.Speed = 1;
                        break;
                    case RBGKeyboardBacklightSpeed.Slow:
                        result.Speed = 2;
                        break;
                    case RBGKeyboardBacklightSpeed.Fast:
                        result.Speed = 3;
                        break;
                    case RBGKeyboardBacklightSpeed.Fastest:
                        result.Speed = 4;
                        break;
                }
            }

            if (preset.Effect == RGBKeyboardBacklightEffect.Static || preset.Effect == RGBKeyboardBacklightEffect.Breath)
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
