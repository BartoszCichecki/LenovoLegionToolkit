using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Controllers
{
    public class SpectrumKeyboardBacklightController
    {
        private static readonly object IoLock = new();

        private readonly Vantage _vantage;

        private SafeFileHandle? DriverHandle => Devices.GetExtendedSpectrumRGBKeyboard() ?? Devices.GetSpectrumRGBKeyboard();

        public SpectrumKeyboardBacklightController(Vantage vantage)
        {
            _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
        }

        public bool IsSupported() => DriverHandle is not null;

        public bool IsExtendedSupported() => IsSupported() && DriverHandle == Devices.GetExtendedSpectrumRGBKeyboard();

        public async Task<int> GetBrightnessAsync()
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_GET_BRIGHTNESS();
            SetAndGetFeature(input, out LENOVO_SPECTRUM_GET_BRIGTHNESS_RESPONSE output);
            return output.Brightness;
        }

        public async Task SetBrightnessAsync(int brightness)
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_SET_BRIGHTHNESS((byte)brightness);
            SetFeature(input);
        }

        public async Task<int> GetProfileAsync()
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_GET_PROFILE();
            SetAndGetFeature(input, out LENOVO_SPECTRUM_GET_PROFILE_RESPONSE output);
            return output.Profile;
        }

        public async Task SetProfileAsync(int profile)
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var input = new LENOVO_SPECTRUM_SET_PROFILE((byte)profile, true);
            SetFeature(input);
        }

        public async Task SetProfileAsync(SpectrumKeyboardBacklightProfile profile, SpectrumKeyboardBacklightProfileDescription description)
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var effects = Convert(profile, description);
            SetFeature(effects);
        }

        public async Task<Dictionary<ushort, RGBColor>> GetStateAsync()
        {
            ThrowIfHandleNull();
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            GetFeature(out LENOVO_SPECTRUM_STATE state);

            var dict = new Dictionary<ushort, RGBColor>();

            foreach (var key in state.Data.Where(k => k.Key > 0))
            {
                var rgb = new RGBColor(key.Color.R, key.Color.G, key.Color.B);
                dict.TryAdd(key.Key, rgb);
            }

            return dict;
        }

        private void ThrowIfHandleNull()
        {
            var handle = DriverHandle;
            if (handle is null)
                throw new InvalidOperationException("Spectrum Keyboard unsupported.");
        }

        private async Task ThrowIfVantageEnabled()
        {
            var vantageStatus = await _vantage.GetStatusAsync().ConfigureAwait(false);
            if (vantageStatus == SoftwareStatus.Enabled)
                throw new InvalidOperationException("Can't manage Spectrum keyboard with Vantage enabled.");
        }

        private void SetAndGetFeature<TIn, TOut>(TIn input, out TOut output) where TIn : struct where TOut : struct
        {
            lock (IoLock)
            {
                SetFeature(input);
                GetFeature(out output);
            }
        }

        private unsafe void SetFeature<T>(T str) where T : struct
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    int size;
                    if (str is ICustomBytesSerializable bs)
                    {
                        var bytes = bs.ToBytes();
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

                    var result = PInvoke.HidD_SetFeature(DriverHandle, ptr.ToPointer(), (uint)size);
                    if (!result)
                        PInvokeExtensions.ThrowIfWin32Error(typeof(T).Name);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        private unsafe void GetFeature<T>(out T str) where T : struct
        {
            lock (IoLock)
            {
                var ptr = IntPtr.Zero;
                try
                {
                    var size = Marshal.SizeOf<T>();
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(new byte[] { 7 }, 0, ptr, 1);

                    var result = PInvoke.HidD_GetFeature(DriverHandle, ptr.ToPointer(), (uint)size);
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

        private static LENOVO_SPECTRUM_SET_EFFECTS Convert(SpectrumKeyboardBacklightProfile profile, SpectrumKeyboardBacklightProfileDescription description)
        {
            var header = new LENOVO_SPECTRUM_HEADER(LENOVO_SPECTRUM_OPERATION_TYPE.EffectChange, 0); // Size will be set on serialization
            var effects = description.Effects.Select((e, i) => Convert(i, e)).ToArray();
            var result = new LENOVO_SPECTRUM_SET_EFFECTS(header, (byte)profile, effects);
            return result;
        }

        private static LENOVO_SPECTRUM_EFFECT Convert(int index, SpectrumKeyboardBacklightEffect effect)
        {
            var effectType = effect.Type switch
            {
                SpectrumKeyboardBacklightEffectType.Always => LENOVO_SPECTRUM_EFFECT_TYPE.Always,
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

            var clockwiseDirection = effect.Direction switch
            {
                SpectrumKeyboardBacklightDirection.Clockwise => LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.Clockwise,
                SpectrumKeyboardBacklightDirection.CounterClockwise => LENOVO_SPECTRUM_CLOCKWISE_DIRECTION.CounterClockwise,
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
                SpectrumKeyboardBacklightEffectType.ColorChange => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.ColorPulse => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.ColorWave => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Rain => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Smooth => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                SpectrumKeyboardBacklightEffectType.Ripple => LENOVO_SPECTRUM_COLOR_MODE.RandomColor,
                _ => LENOVO_SPECTRUM_COLOR_MODE.None
            };

            var header = new LENOVO_SPECTRUM_EFFECT_HEADER(effectType, speed, direction, clockwiseDirection, colorMode);
            var colors = effect.Colors.Select(c => new LENOVO_SPECTRUM_COLOR(c.R, c.G, c.B)).ToArray();
            var result = new LENOVO_SPECTRUM_EFFECT(header, index + 1, colors, effect.Keys);
            return result;
        }
    }
}
