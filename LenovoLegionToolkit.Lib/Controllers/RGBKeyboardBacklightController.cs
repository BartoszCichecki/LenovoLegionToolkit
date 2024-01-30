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
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System.Management;

#if !MOCK_RGB
using System.Runtime.InteropServices;
#endif

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        private static readonly AsyncLock IoLock = new();

        private readonly RGBKeyboardSettings _settings;

        private readonly VantageDisabler _vantageDisabler;

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

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings, VantageDisabler vantageDisabler)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _vantageDisabler = vantageDisabler ?? throw new ArgumentNullException(nameof(vantageDisabler));
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
                    _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                    await ThrowIfVantageEnabled().ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Taking ownership...");

#if !MOCK_RGB
                    await WMI.LenovoGameZoneData.SetLightControlOwnerAsync(enable ? 1 : 0).ConfigureAwait(false);
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
                _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
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
                _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                _settings.Store.State = state;
                _settings.SynchronizeStore();

                var selectedPreset = state.SelectedPreset;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Selected preset: {selectedPreset}");

                LENOVO_RGB_KEYBOARD_STATE str;
                if (selectedPreset == RGBKeyboardBacklightPreset.Off)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating off state.");

                    str = CreateOffState();
                }
                else
                {
                    var presetDescription = state.Presets[selectedPreset];

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating state: {presetDescription}");

                    str = Convert(presetDescription);
                }

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task SetPresetAsync(RGBKeyboardBacklightPreset preset)
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                var state = _settings.Store.State;
                var presets = state.Presets;

                _settings.Store.State = new(preset, presets);
                _settings.SynchronizeStore();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Preset is {preset}.");

                LENOVO_RGB_KEYBOARD_STATE str;
                if (preset == RGBKeyboardBacklightPreset.Off)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating off state.");

                    str = CreateOffState();
                }
                else
                {
                    var presetDescription = state.Presets[preset];

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating state: {presetDescription}");

                    str = Convert(presetDescription);
                }

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task<RGBKeyboardBacklightPreset> SetNextPresetAsync()
        {
            using (await IoLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                var state = _settings.Store.State;

                var newPreset = state.SelectedPreset.Next();
                var presets = state.Presets;

                _settings.Store.State = new(newPreset, presets);
                _settings.SynchronizeStore();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"New preset is {newPreset}.");

                LENOVO_RGB_KEYBOARD_STATE str;
                if (newPreset == RGBKeyboardBacklightPreset.Off)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating off state.");

                    str = CreateOffState();
                }
                else
                {
                    var presetDescription = state.Presets[newPreset];

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Creating state: {presetDescription}");

                    str = Convert(presetDescription);
                }

                await SendToDevice(str).ConfigureAwait(false);

                return newPreset;
            }
        }

        private async Task SetCurrentPresetAsync()
        {
#if !MOCK_RGB
            _ = DeviceHandle ?? throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var state = _settings.Store.State;

            var preset = state.SelectedPreset;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Current preset is {preset}.");

            LENOVO_RGB_KEYBOARD_STATE str;
            if (preset == RGBKeyboardBacklightPreset.Off)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Creating off state.");

                str = CreateOffState();
            }
            else
            {
                var presetDescription = state.Presets[preset];

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Creating state: {presetDescription}");

                str = Convert(presetDescription);
            }

            await SendToDevice(str).ConfigureAwait(false);
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantageDisabler.GetStatusAsync().ConfigureAwait(false);
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

        private static LENOVO_RGB_KEYBOARD_STATE CreateOffState()
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

        private static LENOVO_RGB_KEYBOARD_STATE Convert(RGBKeyboardBacklightBacklightPresetDescription preset)
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
                Effect = preset.Effect switch
                {
                    RGBKeyboardBacklightEffect.Static => 1,
                    RGBKeyboardBacklightEffect.Breath => 3,
                    RGBKeyboardBacklightEffect.WaveRTL => 4,
                    RGBKeyboardBacklightEffect.WaveLTR => 4,
                    RGBKeyboardBacklightEffect.Smooth => 6,
                    _ => 0
                },
                WaveRTL = (byte)(preset.Effect == RGBKeyboardBacklightEffect.WaveRTL ? 1 : 0),
                WaveLTR = (byte)(preset.Effect == RGBKeyboardBacklightEffect.WaveLTR ? 1 : 0),
                Brightness = preset.Brightness switch
                {
                    RGBKeyboardBacklightBrightness.Low => 1,
                    RGBKeyboardBacklightBrightness.High => 2,
                    _ => 0
                }
            };


            if (preset.Effect != RGBKeyboardBacklightEffect.Static)
            {
                result.Speed = preset.Speed switch
                {
                    RGBKeyboardBacklightSpeed.Slowest => 1,
                    RGBKeyboardBacklightSpeed.Slow => 2,
                    RGBKeyboardBacklightSpeed.Fast => 3,
                    RGBKeyboardBacklightSpeed.Fastest => 4,
                    _ => 0
                };
            }

            if (preset.Effect is RGBKeyboardBacklightEffect.Static or RGBKeyboardBacklightEffect.Breath)
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
