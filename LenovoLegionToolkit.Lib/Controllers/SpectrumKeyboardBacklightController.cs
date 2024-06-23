﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32.SafeHandles;
using NeoSmart.AsyncLock;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.Controllers;

public class SpectrumKeyboardBacklightController
{
    public interface IScreenCapture
    {
        void CaptureScreen(ref RGBColor[,] buffer, int width, int height, CancellationToken token);
    }

    private readonly struct KeyMap(int width, int height, ushort[,] keyCodes, ushort[] additionalKeyCodes)
    {
        public static readonly KeyMap Empty = new(0, 0, new ushort[0, 0], []);

        public readonly int Width = width;
        public readonly int Height = height;
        public readonly ushort[,] KeyCodes = keyCodes;
        public readonly ushort[] AdditionalKeyCodes = additionalKeyCodes;
    }

    private static readonly AsyncLock GetDeviceHandleLock = new();
    private static readonly object IoLock = new();

    private readonly TimeSpan _auroraRefreshInterval = TimeSpan.FromMilliseconds(60);

    private readonly SpecialKeyListener _listener;
    private readonly VantageDisabler _vantageDisabler;
    private readonly IScreenCapture _screenCapture;

    private SafeFileHandle? _deviceHandle;

    private CancellationTokenSource? _auroraRefreshCancellationTokenSource;
    private Task? _auroraRefreshTask;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        Converters = [new StringEnumConverter()]
    };

    public bool ForceDisable { get; set; }

    public SpectrumKeyboardBacklightController(SpecialKeyListener listener, VantageDisabler vantageDisabler, IScreenCapture screenCapture)
    {
        _listener = listener;
        _vantageDisabler = vantageDisabler;
        _screenCapture = screenCapture;

        _listener.Changed += Listener_Changed;
    }

    private async void Listener_Changed(object? sender, SpecialKeyListener.ChangedEventArgs e)
    {
        if (!await IsSupportedAsync().ConfigureAwait(false))
            return;

        if (await _vantageDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            return;

        switch (e.SpecialKey)
        {
            case SpecialKey.SpectrumPreset1
                or SpecialKey.SpectrumPreset2
                or SpecialKey.SpectrumPreset3
                or SpecialKey.SpectrumPreset4
                or SpecialKey.SpectrumPreset5
                or SpecialKey.SpectrumPreset6:
                {
                    await StartAuroraIfNeededAsync().ConfigureAwait(false);
                    break;
                }
        }
    }

    public async Task<bool> IsSupportedAsync() => await GetDeviceHandleAsync().ConfigureAwait(false) is not null;

    public async Task<(SpectrumLayout, KeyboardLayout, HashSet<ushort>)> GetKeyboardLayoutAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Checking keyboard layout...");

        var (width, height, keys) = await ReadAllKeyCodesAsync().ConfigureAwait(false);
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        var spectrumLayout = (width, height) switch
        {
            (22, 9) when mi.Properties.HasAlternativeFullSpectrumLayout => SpectrumLayout.FullAlternative,
            (22, 9) => SpectrumLayout.Full,
            (20, 8) => SpectrumLayout.KeyboardAndFront,
            _ => SpectrumLayout.KeyboardOnly // (20, 7)
        };

        var keyboardLayout = keys.Contains(0xA8) ? KeyboardLayout.Iso : KeyboardLayout.Ansi;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Layout is {spectrumLayout}, {keyboardLayout}.");

        return (spectrumLayout, keyboardLayout, keys);
    }

    public async Task<int> GetBrightnessAsync()
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting keyboard brightness...");

        var input = new LENOVO_SPECTRUM_GET_BRIGHTNESS_REQUEST();
        SetAndGetFeature(handle, input, out LENOVO_SPECTRUM_GET_BRIGHTNESS_RESPONSE output);
        var result = output.Brightness;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Keyboard brightness is {result}.");

        return result;
    }

    public async Task SetBrightnessAsync(int brightness)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (brightness is < 0 or > 9)
            throw new InvalidOperationException("Brightness must be between 0 and 9");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting keyboard brightness to: {brightness}.");

        var input = new LENOVO_SPECTRUM_SET_BRIGHTNESS_REQUEST((byte)brightness);
        SetFeature(handle, input);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Keyboard brightness set.");
    }

    public async Task<bool> GetLogoStatusAsync()
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting logo status...");

        var input = new LENOVO_SPECTRUM_GET_LOGO_STATUS();
        SetAndGetFeature(handle, input, out LENOVO_SPECTRUM_GET_LOGO_STATUS_RESPONSE output);
        var result = output.IsOn;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Logo status is {result}.");

        return result;
    }

    public async Task SetLogoStatusAsync(bool isOn)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting logo status to: {isOn}.");

        var input = new LENOVO_SPECTRUM_SET_LOGO_STATUS_REQUEST(isOn);
        SetFeature(handle, input);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Logo status set.");
    }

    public async Task<int> GetProfileAsync()
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting keyboard profile...");

        var input = new LENOVO_SPECTRUM_GET_PROFILE_REQUEST();
        SetAndGetFeature(handle, input, out LENOVO_SPECTRUM_GET_PROFILE_RESPONSE output);
        var result = output.Profile;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Keyboard profile is {result}.");

        return result;
    }

    public async Task SetProfileAsync(int profile)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        await StopAuroraIfNeededAsync().ConfigureAwait(false);

        if (profile is < 0 or > 6)
            throw new InvalidOperationException("Profile must be between 0 and 6");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting keyboard profile to {profile}...");

        var input = new LENOVO_SPECTRUM_SET_PROFILE_REQUEST((byte)profile);
        SetFeature(handle, input);

        await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Keyboard profile set to {profile}.");

        await StartAuroraIfNeededAsync(profile).ConfigureAwait(false);
    }

    public async Task SetProfileDefaultAsync(int profile)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting keyboard profile {profile} to default...");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Keyboard profile {profile} set to default.");

        var input = new LENOVO_SPECTRUM_SET_PROFILE_DEFAULT_REQUEST((byte)profile);
        SetFeature(handle, input);
    }

    public async Task SetProfileDescriptionAsync(int profile, SpectrumKeyboardBacklightEffect[] effects)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting {effects.Length} effect to keyboard profile {profile}...");

        effects = Compress(effects);
        var bytes = Convert(profile, effects).ToBytes();
        SetFeature(handle, bytes);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Set {effects.Length} effect to keyboard profile {profile}.");

        await StartAuroraIfNeededAsync(profile).ConfigureAwait(false);
    }

    public async Task<(int Profile, SpectrumKeyboardBacklightEffect[] Effects)> GetProfileDescriptionAsync(int profile)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting effects for keyboard profile {profile}...");

        var input = new LENOVO_SPECTRUM_GET_EFFECT_REQUEST((byte)profile);
        SetAndGetFeature(handle, input, out var buffer, 960);

        var description = LENOVO_SPECTRUM_EFFECT_DESCRIPTION.FromBytes(buffer);
        var result = Convert(description);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Retrieved {result.Effects.Length} effects for keyboard profile {profile}...");

        return result;
    }

    public async Task ImportProfileDescription(int profile, string jsonPath)
    {
        var json = await File.ReadAllTextAsync(jsonPath).ConfigureAwait(false);
        var effects = JsonConvert.DeserializeObject<SpectrumKeyboardBacklightEffect[]>(json)
                      ?? throw new InvalidOperationException("Couldn't deserialize effects");

        await SetProfileDescriptionAsync(profile, effects).ConfigureAwait(false);
    }

    public async Task ExportProfileDescriptionAsync(int profile, string jsonPath)
    {
        var (_, effects) = await GetProfileDescriptionAsync(profile).ConfigureAwait(false);
        var json = JsonConvert.SerializeObject(effects, _jsonSerializerSettings);
        await File.WriteAllTextAsync(jsonPath, json).ConfigureAwait(false);
    }

    public async Task<bool> StartAuroraIfNeededAsync(int? profile = null)
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        await StopAuroraIfNeededAsync().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting Aurora... [profile={profile}]");

        profile ??= await GetProfileAsync().ConfigureAwait(false);
        var (_, effects) = await GetProfileDescriptionAsync(profile.Value).ConfigureAwait(false);

        if (!effects.Any(e => e.Type == SpectrumKeyboardBacklightEffectType.AuroraSync))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Aurora not needed. [profile={profile}]");

            return false;
        }

        _auroraRefreshCancellationTokenSource = new();
        var token = _auroraRefreshCancellationTokenSource.Token;
        _auroraRefreshTask = Task.Run(() => AuroraRefreshAsync(profile.Value, token), token);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Aurora started. [profile={profile}]");

        return true;
    }

    public async Task StopAuroraIfNeededAsync()
    {
        await ThrowIfVantageEnabled().ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping Aurora...");

        if (_auroraRefreshCancellationTokenSource is not null)
            await _auroraRefreshCancellationTokenSource.CancelAsync().ConfigureAwait(false);

        if (_auroraRefreshTask is not null)
            await _auroraRefreshTask.ConfigureAwait(false);

        _auroraRefreshTask = null;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Aurora stopped.");
    }

    public async Task<Dictionary<ushort, RGBColor>> GetStateAsync(bool skipVantageCheck = false)
    {
        if (!skipVantageCheck)
            await ThrowIfVantageEnabled().ConfigureAwait(false);

        var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
        if (handle is null)
            throw new InvalidOperationException(nameof(handle));

        GetFeature(handle, out LENOVO_SPECTRUM_STATE_RESPONSE state);

        var dict = new Dictionary<ushort, RGBColor>();

        foreach (var key in state.Data.Where(k => k.KeyCode > 0))
        {
            var rgb = new RGBColor(key.Color.R, key.Color.G, key.Color.B);
            dict.TryAdd(key.KeyCode, rgb);
        }

        return dict;
    }

    private async Task ThrowIfVantageEnabled()
    {
        var vantageStatus = await _vantageDisabler.GetStatusAsync().ConfigureAwait(false);
        if (vantageStatus == SoftwareStatus.Enabled)
            throw new InvalidOperationException("Can't manage Spectrum keyboard with Vantage enabled");
    }

    private async Task<(int Width, int Height, HashSet<ushort> Keys)> ReadAllKeyCodesAsync()
    {
        var keyMap = await GetKeyMapAsync().ConfigureAwait(false);
        var keyCodes = new HashSet<ushort>(keyMap.Width * keyMap.Height);

        foreach (var keyCode in keyMap.KeyCodes)
            if (keyCode > 0)
                keyCodes.Add(keyCode);

        foreach (var keyCode in keyMap.AdditionalKeyCodes)
            if (keyCode > 0)
                keyCodes.Add(keyCode);

        return (keyMap.Width, keyMap.Height, keyCodes);
    }

    private async Task<KeyMap> GetKeyMapAsync()
    {
        try
        {
            var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
            if (handle is null)
                return KeyMap.Empty;

            SetAndGetFeature(handle,
                new LENOVO_SPECTRUM_GET_KEY_COUNT_REQUEST(),
                out LENOVO_SPECTRUM_GET_KEY_COUNT_RESPONSE keyCountResponse);

            var width = keyCountResponse.KeysPerIndex;
            var height = keyCountResponse.Indexes;

            var keyCodes = new ushort[width, height];
            var additionalKeyCodes = new ushort[width];

            for (var y = 0; y < height; y++)
            {
                SetAndGetFeature(handle,
                    new LENOVO_SPECTRUM_GET_KEY_PAGE_REQUEST((byte)y),
                    out LENOVO_SPECTRUM_GET_KEY_PAGE_RESPONSE keyPageResponse);

                for (var x = 0; x < width; x++)
                    keyCodes[x, y] = keyPageResponse.Items[x].KeyCode;
            }

            SetAndGetFeature(handle,
                new LENOVO_SPECTRUM_GET_KEY_PAGE_REQUEST(0, true),
                out LENOVO_SPECTRUM_GET_KEY_PAGE_RESPONSE secondaryKeyPageResponse);

            for (var x = 0; x < width; x++)
                additionalKeyCodes[x] = secondaryKeyPageResponse.Items[x].KeyCode;

            return new(width, height, keyCodes, additionalKeyCodes);
        }
        catch
        {
            return KeyMap.Empty;
        }
    }

    private async Task AuroraRefreshAsync(int profile, CancellationToken token)
    {
        try
        {
            await ThrowIfVantageEnabled().ConfigureAwait(false);

            var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
            if (handle is null)
                throw new InvalidOperationException(nameof(handle));

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Aurora refresh starting...");

            var keyMap = await GetKeyMapAsync().ConfigureAwait(false);
            var width = keyMap.Width;
            var height = keyMap.Height;
            var colorBuffer = new RGBColor[width, height];

            SetFeature(handle, new LENOVO_SPECTRUM_AURORA_START_STOP_REQUEST(true, (byte)profile));

            while (!token.IsCancellationRequested)
            {
                var delay = Task.Delay(_auroraRefreshInterval, token);

                try
                {
                    _screenCapture.CaptureScreen(ref colorBuffer, width, height, token);
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Screen capture failed. Delaying before next refresh...", ex);

                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }

                token.ThrowIfCancellationRequested();

                var items = new List<LENOVO_SPECTRUM_AURORA_ITEM>(width * height);

                var avgR = 0;
                var avgG = 0;
                var avgB = 0;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var keyCode = keyMap.KeyCodes[x, y];
                        if (keyCode < 1)
                            continue;

                        var color = colorBuffer[x, y];
                        avgR += color.R;
                        avgG += color.G;
                        avgB += color.B;
                        items.Add(new(keyCode, new(color.R, color.G, color.B)));
                    }
                }

                avgR /= items.Count;
                avgG /= items.Count;
                avgB /= items.Count;

                for (var x = 0; x < width; x++)
                {
                    var keyCode = keyMap.AdditionalKeyCodes[x];
                    if (keyCode < 1)
                        continue;

                    items.Add(new(keyCode, new((byte)avgR, (byte)avgB, (byte)avgG)));
                }

                token.ThrowIfCancellationRequested();

                SetFeature(handle, new LENOVO_SPECTRUM_AURORA_SEND_BITMAP_REQUEST([.. items]).ToBytes());

                await delay.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unexpected exception while refreshing Aurora.", ex);
        }
        finally
        {
            var handle = await GetDeviceHandleAsync().ConfigureAwait(false);
            if (handle is not null)
            {
                var currentProfile = await GetProfileAsync().ConfigureAwait(false);
                SetFeature(handle, new LENOVO_SPECTRUM_AURORA_START_STOP_REQUEST(false, (byte)currentProfile));
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Aurora refresh stopped.");
        }
    }

    private async Task<SafeFileHandle?> GetDeviceHandleAsync()
    {
        if (ForceDisable)
            return null;

        try
        {
            using (await GetDeviceHandleLock.LockAsync().ConfigureAwait(false))
            {
                if (_deviceHandle is not null && IsReady(_deviceHandle))
                    return _deviceHandle;

                SafeFileHandle? newDeviceHandle = null;

                const int retries = 3;
                const int delay = 50;

                for (var i = 0; i < retries; i++)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Refreshing handle... [retry={i + 1}]");

                    var tempDeviceHandle = Devices.GetSpectrumRGBKeyboard(true);
                    if (tempDeviceHandle is not null && IsReady(tempDeviceHandle))
                    {
                        newDeviceHandle = tempDeviceHandle;
                        break;
                    }

                    await Task.Delay(delay).ConfigureAwait(false);
                }

                if (newDeviceHandle is null)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Handle couldn't be refreshed.");

                    return null;
                }

                SetAndGetFeature(newDeviceHandle,
                    new LENOVO_SPECTRUM_GET_COMPATIBILITY_REQUEST(),
                    out LENOVO_SPECTRUM_GET_COMPATIBILITY_RESPONSE res);

                if (!res.IsCompatible)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Handle not compatible.");

                    return null;
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Handle refreshed.");

                _deviceHandle = newDeviceHandle;
                return newDeviceHandle;
            }
        }
        catch
        {
            return null;
        }
    }

    private static bool IsReady(SafeHandle handle)
    {
        try
        {
            var b = new byte[960];
            b[0] = 7;
            SetFeature(handle, b);
            return true;
        }
        catch
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Keyboard not ready.");

            return false;
        }
    }

    private static void SetAndGetFeature<TIn, TOut>(SafeHandle handle, TIn input, out TOut output) where TIn : notnull where TOut : struct
    {
        lock (IoLock)
        {
            SetFeature(handle, input);
            GetFeature(handle, out output);
        }
    }

    private static void SetAndGetFeature<TIn>(SafeHandle handle, TIn input, out byte[] output, int size) where TIn : notnull
    {
        lock (IoLock)
        {
            SetFeature(handle, input);
            GetFeature(handle, out output, size);
        }
    }

    private static unsafe void SetFeature<T>(SafeHandle handle, T str) where T : notnull
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

    private static unsafe void GetFeature<T>(SafeHandle handle, out T str) where T : struct
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

    private static unsafe void GetFeature(SafeHandle handle, out byte[] bytes, int size)
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
        if (effects.Any(e => e.Type.IsAllLightsEffect()))
            return [effects.Last(e => e.Type.IsAllLightsEffect())];

        var usedKeyCodes = new HashSet<ushort>();
        var newEffects = new List<SpectrumKeyboardBacklightEffect>();

        foreach (var effect in effects.Reverse())
        {
            if (effect.Type.IsWholeKeyboardEffect() && usedKeyCodes.Intersect(effect.Keys).Any())
                continue;

            var newKeyCodes = effect.Keys.Except(usedKeyCodes).ToArray();

            foreach (var keyCode in newKeyCodes)
                usedKeyCodes.Add(keyCode);

            if (newKeyCodes.IsEmpty())
                continue;

            var newEffect = new SpectrumKeyboardBacklightEffect(effect.Type,
                effect.Speed,
                effect.Direction,
                effect.ClockwiseDirection,
                effect.Colors,
                newKeyCodes);

            newEffects.Add(newEffect);
        }

        newEffects.Reverse();
        return [.. newEffects];
    }

    private static (int Profile, SpectrumKeyboardBacklightEffect[] Effects) Convert(LENOVO_SPECTRUM_EFFECT_DESCRIPTION description)
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
            _ => throw new ArgumentException(nameof(effect.EffectHeader.EffectType))
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

        var keys = effect.KeyCodes;
        if (effect.KeyCodes is [0x65])
            keys = [];

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
            _ => throw new ArgumentException(nameof(effect.Type))
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
            SpectrumKeyboardBacklightEffectType.ColorChange when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.ColorPulse when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.ColorWave when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.Rain when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.Smooth when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.Ripple when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
            SpectrumKeyboardBacklightEffectType.Type when effect.Colors.Length != 0 => LENOVO_SPECTRUM_COLOR_MODE.ColorList,
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
        var keys = effect.Type.IsAllLightsEffect() ? [0x65] : effect.Keys;
        var result = new LENOVO_SPECTRUM_EFFECT(header, index + 1, colors, keys);
        return result;
    }
}
