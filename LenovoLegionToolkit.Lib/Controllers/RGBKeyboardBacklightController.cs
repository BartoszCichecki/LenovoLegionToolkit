// #define MOCK_RGB

using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using NeoSmart.AsyncLock;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using LenovoLegionToolkit.Lib.Listeners;

#if !MOCK_RGB
using System.Runtime.InteropServices;
#endif

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class RGBKeyboardBacklightController
    {
        public bool ForceDisable { get; set; } = false;

        private readonly AsyncLock _ioLock = new();

        private readonly RGBKeyboardSettings _settings;

        private readonly Vantage _vantage;

        private readonly SystemThemeListener _systemThemeListener;

        private SafeFileHandle? DriverHandle
        {
            get
            {
                if (ForceDisable)
                    return null;

                return Devices.GetRGBKeyboard();
            }
        }

        public RGBKeyboardBacklightController(RGBKeyboardSettings settings, Vantage vantage, SystemThemeListener listener)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
            _systemThemeListener = listener ?? throw new ArgumentNullException(nameof(listener));

            _systemThemeListener.Changed += SystemThemeListener_Changed;
        }

        public bool IsSupported()
        {
#if MOCK_RGB
            return true;
#else
            return DriverHandle is not null;
#endif
        }

        public async Task SetLightControlOwnerAsync(bool enable, bool restorePreset = false)
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                try
                {
#if !MOCK_RGB
                    var handle = DriverHandle;
                    if (handle is null)
                        throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                    await ThrowIfVantageEnabled().ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Taking ownership...");

#if !MOCK_RGB
                    await WMI.CallAsync("ROOT\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_DATA", "SetLightControlOwner", new() { { "Data", enable ? 1 : 0 } }).ConfigureAwait(false);
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
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DriverHandle;
                if (handle is null)
                    throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                return _settings.Store.State;
            }
        }

        public async Task SetStateAsync(RGBKeyboardBacklightState state, RGBColor? systemAccentColor = null)
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
#if !MOCK_RGB
                var handle = DriverHandle;
                if (handle is null)
                    throw new InvalidOperationException("RGB Keyboard unsupported.");
#endif

                await ThrowIfVantageEnabled().ConfigureAwait(false);

                _settings.Store.State = state;
                _settings.SynchronizeStore();

                var selectedPreset = state.SelectedPreset;

                RGBKeyboardStateEx str;
                if (selectedPreset == RGBKeyboardBacklightPreset.Off)
                    str = CreateOffState();
                else
                {
                    RGBKeyboardBacklightSettings settings = state.Presets[selectedPreset];

                    if (systemAccentColor == null)
                        systemAccentColor = SystemTheme.GetAccentColor();

                    settings = new RGBKeyboardBacklightSettings(settings.Effect, settings.Speed, settings.Brightness,
                                     settings.Zone1.SyncSystemAccentColor ? new RGBKeyboardZone(systemAccentColor.Value, true) : settings.Zone1,
                                     settings.Zone2.SyncSystemAccentColor ? new RGBKeyboardZone(systemAccentColor.Value, true) : settings.Zone2,
                                     settings.Zone3.SyncSystemAccentColor ? new RGBKeyboardZone(systemAccentColor.Value, true) : settings.Zone3,
                                     settings.Zone4.SyncSystemAccentColor ? new RGBKeyboardZone(systemAccentColor.Value, true) : settings.Zone4);

                    str = Convert(settings);
                }

                await SendToDevice(str).ConfigureAwait(false);
            }
        }

        public async Task SetPresetAsync(RGBKeyboardBacklightPreset preset, RGBColor? systemAccentColor = null)
        {
            await SetStateAsync(new(preset, _settings.Store.State.Presets), systemAccentColor).ConfigureAwait(false);
        }

        public async Task SetNextPresetAsync()
        {
            await SetPresetAsync(_settings.Store.State.SelectedPreset.Next()).ConfigureAwait(false);
        }

        private async Task SetCurrentPresetAsync()
        {
            await SetPresetAsync(_settings.Store.State.SelectedPreset).ConfigureAwait(false);
        }

        private async void SystemThemeListener_Changed(object? sender, SystemThemeSettings e)
        {
            await SetPresetAsync(_settings.Store.State.SelectedPreset, e.AccentColor).ConfigureAwait(false);
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't manage RGB keyboard with Vantage enabled.");
        }

        private Task SendToDevice(RGBKeyboardStateEx str) => Task.Run(() =>
        {
#if !MOCK_RGB
            var handle = DriverHandle;
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

        private RGBKeyboardStateEx CreateOffState()
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

        private RGBKeyboardStateEx Convert(RGBKeyboardBacklightSettings preset)
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
                result.Zone1Rgb = new[] { preset.Zone1.Color.R, preset.Zone1.Color.G, preset.Zone1.Color.B };
                result.Zone2Rgb = new[] { preset.Zone2.Color.R, preset.Zone2.Color.G, preset.Zone2.Color.B };
                result.Zone3Rgb = new[] { preset.Zone3.Color.R, preset.Zone3.Color.G, preset.Zone3.Color.B };
                result.Zone4Rgb = new[] { preset.Zone4.Color.R, preset.Zone4.Color.G, preset.Zone4.Color.B };
            }

            return result;
        }
    }
}
