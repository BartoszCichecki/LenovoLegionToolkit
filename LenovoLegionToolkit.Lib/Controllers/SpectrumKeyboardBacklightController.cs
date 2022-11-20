using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class SpectrumKeyboardBacklightController
    {
        public interface IScreenCapture
        {
            RGBColor[][] CaptureScreen(int width, int height);
        }

        private static readonly object IoLock = new();

        private readonly SpecialKeyListener _listener;
        private readonly Vantage _vantage;
        private readonly IScreenCapture _screenCapture;

        private readonly Lazy<SafeFileHandle?> _driverHandle;
        private readonly Lazy<bool> _isExtended;

        private CancellationTokenSource? _auroraRefreshCancellationTokenSource;
        private Task? _auroraRefreshTask;

        private SafeFileHandle? DriverHandle => _driverHandle.Value;

        public bool IsExtended => _isExtended.Value;

        public bool ForceDisable { get; set; }

        public SpectrumKeyboardBacklightController(SpecialKeyListener listener, Vantage vantage, IScreenCapture screenCapture)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
            _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
            _screenCapture = screenCapture ?? throw new ArgumentNullException(nameof(screenCapture));

            _driverHandle = new(HandleValueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _isExtended = new(IsExtendedValueFactory, LazyThreadSafetyMode.ExecutionAndPublication);

            _listener.Changed += Listener_Changed;
        }

        private SafeFileHandle? HandleValueFactory()
        {
            try
            {
                if (ForceDisable)
                    return null;

                var handle = Devices.GetSpectrumRGBKeyboard();
                if (handle is null)
                    return null;

                SetAndGetFeature(handle, new LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST(), out LENOVO_SPECTRUM_GET_COMPATIBILITY_RESPONSE res);
                if (!res.IsCompatible)
                    return null;

                return handle;
            }
            catch
            {
                return null;
            }
        }

        private bool IsExtendedValueFactory()
        {
            try
            {
                if (DriverHandle is null)
                    return false;

                SetAndGetFeature(DriverHandle, new LENOVO_SPECTRUM_GET_KEYCOUNT_REQUEST(), out LENOVO_SPECTRUM_GET_KEYCOUNT_RESPONSE res);
                return res.IsExtended;
            }
            catch
            {
                return false;
            }
        }

        private async void Listener_Changed(object? sender, SpecialKey e)
        {
            if (!IsSupported() || await _vantage.GetStatusAsync() == SoftwareStatus.Enabled)
                return;

            switch (e)
            {
                case SpecialKey.SpectrumPreset1
                    or SpecialKey.SpectrumPreset2
                    or SpecialKey.SpectrumPreset3
                    or SpecialKey.SpectrumPreset4
                    or SpecialKey.SpectrumPreset5
                    or SpecialKey.SpectrumPreset6:
                    {
                        if (_auroraRefreshTask is null)
                            await StartAuroraIfNeededAsync().ConfigureAwait(false);
                        else
                            await StopAuroraIfNeededAsync().ConfigureAwait(false);
                        break;
                    }
            }
        }

        public bool IsSupported() => DriverHandle is not null;

        public KeyboardLayout GetKeyboardLayout()
        {
            var keys = ReadAllKeyCodes();
            return keys.Contains(0xA8) ? KeyboardLayout.Iso : KeyboardLayout.Ansi;
        }

        public async Task<int> GetBrightnessAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST();
            SetAndGetFeature(DriverHandle, input, out LENOVO_SPECTRUM_GET_BRIGTHNESS_RESPONSE output);
            return output.Brightness;
        }

        public async Task SetBrightnessAsync(int brightness)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_SET_BRIGHTHNESS_REQUEST((byte)brightness);
            SetFeature(DriverHandle, input);
        }

        public async Task<int> GetProfileAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_GET_PROFILE_REQUEST();
            SetAndGetFeature(DriverHandle, input, out LENOVO_SPECTRUM_GET_PROFILE_RESPONSE output);
            return output.Profile;
        }

        public async Task SetProfileAsync(int profile)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_SET_PROFILE_REQUEST((byte)profile);
            SetFeature(DriverHandle, input);

            // Looks like keyboard needs some time sometimes
            int currentProfile;
            do
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                currentProfile = await GetProfileAsync().ConfigureAwait(false);
            } while (currentProfile != profile);

            await StartAuroraIfNeededAsync(profile).ConfigureAwait(false);
        }

        public async Task SetProfileDefaultAsync(int profile)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST((byte)profile);
            SetFeature(DriverHandle, input);
        }

        public async Task SetProfileDescriptionAsync(int profile, SpectrumKeyboardBacklightEffect[] effects)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            effects = Compress(effects);
            var bytes = Convert(profile, effects).ToBytes();
            SetFeature(DriverHandle, bytes);

            await StopAuroraIfNeededAsync().ConfigureAwait(false);
            await StartAuroraIfNeededAsync(profile).ConfigureAwait(false);
        }

        public async Task<(int, SpectrumKeyboardBacklightEffect[])> GetProfileDescriptionAsync(int profile)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            var input = new LENOVO_SPECTRUM_GET_EFFECT_REQUEST((byte)profile);
            SetAndGetFeature(DriverHandle, input, out var buffer, 960);

            var description = LENOVO_SPECTRUM_EFFECT_DESCRIPTION.FromBytes(buffer);
            return Convert(description);
        }

        public async Task<bool> StartAuroraIfNeededAsync(int? profile = null)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            await StopAuroraIfNeededAsync().ConfigureAwait(false);

            profile ??= await GetProfileAsync().ConfigureAwait(false);
            var (_, description) = await GetProfileDescriptionAsync(profile.Value).ConfigureAwait(false);

            if (!description.Any(e => e.Type == SpectrumKeyboardBacklightEffectType.AuroraSync))
                return false;

            _auroraRefreshCancellationTokenSource = new();
            _auroraRefreshTask = Task.Run(() => AuroraRefreshAsync(profile.Value, _auroraRefreshCancellationTokenSource.Token));

            return true;

        }

        public async Task StopAuroraIfNeededAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            _auroraRefreshCancellationTokenSource?.Cancel();
            if (_auroraRefreshTask is not null)
                await _auroraRefreshTask.ConfigureAwait(false);
            _auroraRefreshTask = null;
        }

        public async Task<Dictionary<ushort, RGBColor>> GetStateAsync()
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            GetFeature(DriverHandle, out LENOVO_SPECTRUM_STATE_RESPONSE state);

            var dict = new Dictionary<ushort, RGBColor>();

            foreach (var key in state.Data.Where(k => k.Key > 0))
            {
                var rgb = new RGBColor(key.Color.R, key.Color.G, key.Color.B);
                dict.TryAdd(key.Key, rgb);
            }

            return dict;
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't manage Spectrum keyboard with Vantage enabled.");
        }

        private HashSet<ushort> ReadAllKeyCodes() => GetKeyMap().SelectMany(k => k).Where(k => k > 0).ToHashSet();

        private ushort[][] GetKeyMap()
        {
            try
            {
                if (DriverHandle is null)
                    return Array.Empty<ushort[]>();

                SetAndGetFeature(DriverHandle, new LENOVO_SPECTRUM_GET_KEYCOUNT_REQUEST(), out LENOVO_SPECTRUM_GET_KEYCOUNT_RESPONSE keyCountResponse);

                var keyMap = new ushort[keyCountResponse.Indexes][];

                for (var i = 0; i < keyCountResponse.Indexes; i++)
                {
                    SetAndGetFeature(DriverHandle, new LENOVO_SPECTRUM_GET_KEYPAGE_REQUEST((byte)i), out LENOVO_SPECTRUM_GET_KEYPAGE_RESPONSE keyPageResponse);
                    keyMap[i] = keyPageResponse.Items.Take(keyCountResponse.KeysPerIndex).Select(k => k.Key).ToArray();
                }

                return keyMap;
            }
            catch
            {
                return Array.Empty<ushort[]>();
            }
        }

        private async void AuroraRefreshAsync(int profile, CancellationToken token)
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            if (DriverHandle is null)
                throw new InvalidOperationException(nameof(DriverHandle));

            try
            {
                var keyMap = GetKeyMap();
                var width = keyMap[0].Length;
                var height = keyMap.Length;

                SetFeature(DriverHandle, new LENOVO_SPECTRUM_AURORA_STARTSTOP_REQUEST(true, (byte)profile));

                while (!token.IsCancellationRequested)
                {
                    var delay = Task.Delay(100, token);

                    var bitmap = _screenCapture.CaptureScreen(width, height);

                    var items = new List<LENOVO_SEPCTRUM_AURORA_ITEM>();

                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var keyCode = keyMap[y][x];
                            if (keyCode < 1)
                                continue;
                            var color = bitmap[y][x];
                            items.Add(new(keyCode, new(color.R, color.G, color.B)));
                        }
                    }

                    SetFeature(DriverHandle, new LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST(items.ToArray()).ToBytes());

                    await delay;
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Unexpected exception in while sending bitmap.", ex);
            }
            finally
            {
                var currentProfile = await GetProfileAsync();
                SetFeature(DriverHandle, new LENOVO_SPECTRUM_AURORA_STARTSTOP_REQUEST(false, (byte)currentProfile));
            }
        }

        private void SetAndGetFeature<TIn, TOut>(SafeFileHandle handle, TIn input, out TOut output) where TIn : notnull where TOut : struct
        {
            lock (IoLock)
            {
                SetFeature(handle, input);
                GetFeature(handle, out output);
            }
        }

        private void SetAndGetFeature<TIn>(SafeFileHandle handle, TIn input, out byte[] output, int size) where TIn : notnull
        {
            lock (IoLock)
            {
                SetFeature(handle, input);
                GetFeature(handle, out output, size);
            }
        }

        private unsafe void SetFeature<T>(SafeFileHandle handle, T str) where T : notnull
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    int size;
                    if (str is byte[] bytes)
                    {
                        size = bytes.Length;
                        ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(bytes, 0, ptr, size);
                    }
                    else
                    {
                        size = Marshal.SizeOf<T>();
                        ptr = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(str, ptr, false);
                    }

                    var result = PInvoke.HidD_SetFeature(handle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private unsafe void GetFeature<T>(SafeFileHandle handle, out T str) where T : struct
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf<T>();
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(new byte[] { 7 }, 0, ptr, 1);

                    var result = PInvoke.HidD_GetFeature(handle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);

                    str = Marshal.PtrToStructure<T>(ptr);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private unsafe void GetFeature(SafeFileHandle handle, out byte[] bytes, int size)
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(new byte[] { 7 }, 0, ptr, 1);

                    var result = PInvoke.HidD_GetFeature(handle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error("bytes");

                    bytes = new byte[size];
                    Marshal.Copy(ptr, bytes, 0, size);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private static SpectrumKeyboardBacklightEffect[] Compress(SpectrumKeyboardBacklightEffect[] effects)
        {
            if (effects.Any(e => e.Keys.All))
                return new[] { effects.Last(e => e.Keys.All) };

            var usedKeyCodes = new HashSet<ushort>();
            var newEffects = new List<SpectrumKeyboardBacklightEffect>();

            foreach (var effect in effects.Reverse())
            {
                var newKeyCodes = effect.Keys.KeyCodes.Except(usedKeyCodes).ToArray();

                foreach (var keyCode in newKeyCodes)
                    usedKeyCodes.Add(keyCode);

                if (newKeyCodes.IsEmpty())
                    continue;

                var newEffect = new SpectrumKeyboardBacklightEffect(effect.Type,
                    effect.Speed,
                    effect.Direction,
                    effect.ClockwiseDirection,
                    effect.Colors,
                    SpectrumKeyboardBacklightKeys.SomeKeys(newKeyCodes));

                newEffects.Add(newEffect);
            }

            return newEffects.ToArray();
        }

        private static (int, SpectrumKeyboardBacklightEffect[]) Convert(LENOVO_SPECTRUM_EFFECT_DESCRIPTION description)
        {
            var profile = description.Profile;
            var effects = description.Effects.Select(Convert).ToArray();
            return (profile, effects);
        }

        private static SpectrumKeyboardBacklightEffect Convert(LENOVO_SPECTRUM_EFFECT effect)
        {
            var effectType = effect.EffectHeader.EffectType switch
            {
                LENOVO_SPECTRUM_EFFECT_TYPE.Always => SpectrumKeyboardBacklightEffectType.Always,
                LENOVO_SPECTRUM_EFFECT_TYPE.LegionAuraSync => SpectrumKeyboardBacklightEffectType.AuroraSync,
                LENOVO_SPECTRUM_EFFECT_TYPE.AudioBounceLighting => SpectrumKeyboardBacklightEffectType.AudioBounce,
                LENOVO_SPECTRUM_EFFECT_TYPE.AudioRippleLighting => SpectrumKeyboardBacklightEffectType.AudioRipple,
                LENOVO_SPECTRUM_EFFECT_TYPE.ColorChange => SpectrumKeyboardBacklightEffectType.ColorChange,
                LENOVO_SPECTRUM_EFFECT_TYPE.ColorPulse => SpectrumKeyboardBacklightEffectType.ColorPulse,
                LENOVO_SPECTRUM_EFFECT_TYPE.ColorWave => SpectrumKeyboardBacklightEffectType.ColorWave,
                LENOVO_SPECTRUM_EFFECT_TYPE.Rain => SpectrumKeyboardBacklightEffectType.Rain,
                LENOVO_SPECTRUM_EFFECT_TYPE.ScrewRainbow => SpectrumKeyboardBacklightEffectType.RainbowScrew,
                LENOVO_SPECTRUM_EFFECT_TYPE.RainbowWave => SpectrumKeyboardBacklightEffectType.RainbowWave,
                LENOVO_SPECTRUM_EFFECT_TYPE.Ripple => SpectrumKeyboardBacklightEffectType.Ripple,
                LENOVO_SPECTRUM_EFFECT_TYPE.Smooth => SpectrumKeyboardBacklightEffectType.Smooth,
                LENOVO_SPECTRUM_EFFECT_TYPE.TypeLighting => SpectrumKeyboardBacklightEffectType.Type,
                _ => throw new ArgumentException()
            };

            var speed = effect.EffectHeader.Speed switch
            {
                LENOVO_SPECTRUM_SPEED.Speed1 => SpectrumKeyboardBacklightSpeed.Speed1,
                LENOVO_SPECTRUM_SPEED.Speed2 => SpectrumKeyboardBacklightSpeed.Speed2,
                LENOVO_SPECTRUM_SPEED.Speed3 => SpectrumKeyboardBacklightSpeed.Speed3,
                _ => SpectrumKeyboardBacklightSpeed.None
            };

            var direction = effect.EffectHeader.Direction switch
            {
                LENOVO_SPECTRUM_DIRECTION.LeftToRight => SpectrumKeyboardBacklightDirection.LeftToRight,
                LENOVO_SPECTRUM_DIRECTION.RightToLeft => SpectrumKeyboardBacklightDirection.RightToLeft,
                LENOVO_SPECTRUM_DIRECTION.BottomToTop => SpectrumKeyboardBacklightDirection.BottomToTop,
                LENOVO_SPECTRUM_DIRECTION.TopToBottom => SpectrumKeyboardBacklightDirection.TopToBottom,
                _ => SpectrumKeyboardBacklightDirection.None
            };

            var clockwiseDirection = effect.EffectHeader.ClockwiseDirection switch
            {
                LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.Clockwise => SpectrumKeyboardBacklightClockwiseDirection.Clockwise,
                LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.CounterClockwise => SpectrumKeyboardBacklightClockwiseDirection.CounterClockwise,
                _ => SpectrumKeyboardBacklightClockwiseDirection.None
            };

            var colors = effect.Colors.Select(c => new RGBColor(c.R, c.G, c.B)).ToArray();

            SpectrumKeyboardBacklightKeys keys;
            if (effect.Keys.Length == 1 && effect.Keys[0] == 0x65)
                keys = SpectrumKeyboardBacklightKeys.AllKeys();
            else
                keys = SpectrumKeyboardBacklightKeys.SomeKeys(effect.Keys);

            return new(effectType, speed, direction, clockwiseDirection, colors, keys);
        }

        private static LENOVO_SPECTRUM_EFFECT_DESCRIPTION Convert(int profile, SpectrumKeyboardBacklightEffect[] effects)
        {
            var header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.EffectChange, 0); // Size will be set on serialization
            var str = effects.Select((e, i) => Convert(i, e)).ToArray();
            var result = new LENOVO_SPECTRUM_EFFECT_DESCRIPTION(header, (byte)profile, str);
            return result;
        }

        private static LENOVO_SPECTRUM_EFFECT Convert(int index, SpectrumKeyboardBacklightEffect effect)
        {
            var effectType = effect.Type switch
            {
                SpectrumKeyboardBacklightEffectType.Always => LENOVO_SPECTRUM_EFFECT_TYPE.Always,
                SpectrumKeyboardBacklightEffectType.AuroraSync => LENOVO_SPECTRUM_EFFECT_TYPE.LegionAuraSync,
                SpectrumKeyboardBacklightEffectType.AudioBounce => LENOVO_SPECTRUM_EFFECT_TYPE.AudioBounceLighting,
                SpectrumKeyboardBacklightEffectType.AudioRipple => LENOVO_SPECTRUM_EFFECT_TYPE.AudioRippleLighting,
                SpectrumKeyboardBacklightEffectType.ColorChange => LENOVO_SPECTRUM_EFFECT_TYPE.ColorChange,
                SpectrumKeyboardBacklightEffectType.ColorPulse => LENOVO_SPECTRUM_EFFECT_TYPE.ColorPulse,
                SpectrumKeyboardBacklightEffectType.ColorWave => LENOVO_SPECTRUM_EFFECT_TYPE.ColorWave,
                SpectrumKeyboardBacklightEffectType.Rain => LENOVO_SPECTRUM_EFFECT_TYPE.Rain,
                SpectrumKeyboardBacklightEffectType.RainbowScrew => LENOVO_SPECTRUM_EFFECT_TYPE.ScrewRainbow,
                SpectrumKeyboardBacklightEffectType.RainbowWave => LENOVO_SPECTRUM_EFFECT_TYPE.RainbowWave,
                SpectrumKeyboardBacklightEffectType.Ripple => LENOVO_SPECTRUM_EFFECT_TYPE.Ripple,
                SpectrumKeyboardBacklightEffectType.Smooth => LENOVO_SPECTRUM_EFFECT_TYPE.Smooth,
                SpectrumKeyboardBacklightEffectType.Type => LENOVO_SPECTRUM_EFFECT_TYPE.TypeLighting,
                _ => throw new ArgumentException()
            };

            var speed = effect.Speed switch
            {
                SpectrumKeyboardBacklightSpeed.Speed1 => LENOVO_SPECTRUM_SPEED.Speed1,
                SpectrumKeyboardBacklightSpeed.Speed2 => LENOVO_SPECTRUM_SPEED.Speed2,
                SpectrumKeyboardBacklightSpeed.Speed3 => LENOVO_SPECTRUM_SPEED.Speed3,
                _ => LENOVO_SPECTRUM_SPEED.None
            };

            var direction = effect.Direction switch
            {
                SpectrumKeyboardBacklightDirection.LeftToRight => LENOVO_SPECTRUM_DIRECTION.LeftToRight,
                SpectrumKeyboardBacklightDirection.RightToLeft => LENOVO_SPECTRUM_DIRECTION.RightToLeft,
                SpectrumKeyboardBacklightDirection.BottomToTop => LENOVO_SPECTRUM_DIRECTION.BottomToTop,
                SpectrumKeyboardBacklightDirection.TopToBottom => LENOVO_SPECTRUM_DIRECTION.TopToBottom,
                _ => LENOVO_SPECTRUM_DIRECTION.None
            };

            var clockwiseDirection = effect.ClockwiseDirection switch
            {
                SpectrumKeyboardBacklightClockwiseDirection.Clockwise => LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.Clockwise,
                SpectrumKeyboardBacklightClockwiseDirection.CounterClockwise => LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.CounterClockwise,
                _ => LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.None
            };

            var colorMode = effect.Type switch
            {
                SpectrumKeyboardBacklightEffectType.Always => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.ColorChange when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.ColorPulse when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.ColorWave when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.Rain when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.Smooth when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.Ripple when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.Type when effect.Colors.Any() => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
                SpectrumKeyboardBacklightEffectType.ColorChange => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.ColorPulse => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.ColorWave => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Rain => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Smooth => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Ripple => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Type => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                _ => LENOVO_SPECTRUM_COLOR_MODE.None
            };

            var header = new LENOVO_SPECTRUM_EFFECT_HEADER(effectType, speed, direction, clockwiseDirection, colorMode);
            var colors = effect.Colors.Select(c => new LENOVO_SPECTRUM_COLOR(c.R, c.G, c.B)).ToArray();
            var keys = effect.Keys.All ? new ushort[] { 0x65 } : effect.Keys.KeyCodes;
            var result = new LENOVO_SPECTRUM_EFFECT(header, index + 1, colors, keys);
            return result;
        }
    }
}
