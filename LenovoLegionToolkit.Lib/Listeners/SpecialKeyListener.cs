﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class SpecialKeyListener : AbstractWMIListener<SpecialKeyListener.ChangedEventArgs, SpecialKey, int>
{
    public class ChangedEventArgs : EventArgs
    {
        public SpecialKey SpecialKey { get; init; }
    }

    private readonly ThrottleFirstDispatcher _refreshRateDispatcher = new(TimeSpan.FromSeconds(2), nameof(SpecialKeyListener));

    private readonly ApplicationSettings _settings;
    private readonly FnKeysDisabler _fnKeysDisabler;
    private readonly RefreshRateFeature _refreshRateFeature;
    private readonly MicrophoneFeature _microphoneFeature;

    public SpecialKeyListener(ApplicationSettings settings, FnKeysDisabler fnKeysDisabler, RefreshRateFeature feature, MicrophoneFeature microphoneFeature)
        : base(WMI.LenovoUtilityEvent.Listen)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fnKeysDisabler = fnKeysDisabler ?? throw new ArgumentNullException(nameof(fnKeysDisabler));
        _refreshRateFeature = feature ?? throw new ArgumentNullException(nameof(feature));
        _microphoneFeature = microphoneFeature ?? throw new ArgumentNullException(nameof(microphoneFeature));
    }

    protected override SpecialKey GetValue(int value)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [value={value}]");

        var result = (SpecialKey)value;
        return result;
    }

    protected override ChangedEventArgs GetEventArgs(SpecialKey value) => new() { SpecialKey = value };

    protected override async Task OnChangedAsync(SpecialKey value)
    {
        try
        {
            if (await _fnKeysDisabler.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");
                return;
            }

            switch (value)
            {
                case SpecialKey.CameraOn or SpecialKey.CameraOff:
                    NotifyCameraState(value);
                    break;
                case SpecialKey.FnLockOn or SpecialKey.FnLockOff:
                    NotifyFnLockState(value);
                    break;
                case SpecialKey.FnR or SpecialKey.FnR2:
                    await ToggleRefreshRateAsync().ConfigureAwait(false);
                    break;
                case SpecialKey.FnPrtSc or SpecialKey.FnPrtSc2:
                    OpenSnippingTool();
                    break;
                case SpecialKey.SpectrumBacklightOff:
                    NotifySpectrumBacklight(SpectrumKeyboardBacklightBrightness.Off);
                    break;
                case SpecialKey.SpectrumBacklight1:
                    NotifySpectrumBacklight(SpectrumKeyboardBacklightBrightness.Low);
                    break;
                case SpecialKey.SpectrumBacklight2:
                    NotifySpectrumBacklight(SpectrumKeyboardBacklightBrightness.Medium);
                    break;
                case SpecialKey.SpectrumBacklight3:
                    NotifySpectrumBacklight(SpectrumKeyboardBacklightBrightness.High);
                    break;
                case SpecialKey.SpectrumPreset1:
                    NotifySpectrumPreset(1);
                    break;
                case SpecialKey.SpectrumPreset2:
                    NotifySpectrumPreset(2);
                    break;
                case SpecialKey.SpectrumPreset3:
                    NotifySpectrumPreset(3);
                    break;
                case SpecialKey.SpectrumPreset4:
                    NotifySpectrumPreset(4);
                    break;
                case SpecialKey.SpectrumPreset5:
                    NotifySpectrumPreset(5);
                    break;
                case SpecialKey.SpectrumPreset6:
                    NotifySpectrumPreset(6);
                    break;
                case SpecialKey.FnF4:
                    await ToggleMicrophoneAsync().ConfigureAwait(false);
                    break;
                case SpecialKey.FnF8:
                    OpenAirplaneModeSettings();
                    break;
                case SpecialKey.WhiteBacklightOff:
                    NotifyWhiteBacklight(WhiteKeyboardBacklightState.Off);
                    break;
                case SpecialKey.WhiteBacklight1:
                    NotifyWhiteBacklight(WhiteKeyboardBacklightState.Low);
                    break;
                case SpecialKey.WhiteBacklight2:
                    NotifyWhiteBacklight(WhiteKeyboardBacklightState.High);
                    break;
            }
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to handle key. [key={value}, value={(int)value}]", ex);
        }
    }

    private static void NotifyCameraState(SpecialKey value)
    {
        switch (value)
        {
            case SpecialKey.CameraOn:
                MessagingCenter.Publish(new Notification(NotificationType.CameraOn));
                break;
            case SpecialKey.CameraOff:
                MessagingCenter.Publish(new Notification(NotificationType.CameraOff));
                break;
        }
    }

    private static void NotifyFnLockState(SpecialKey value)
    {
        switch (value)
        {
            case SpecialKey.FnLockOn:
                MessagingCenter.Publish(new Notification(NotificationType.FnLockOn));
                break;
            case SpecialKey.FnLockOff:
                MessagingCenter.Publish(new Notification(NotificationType.FnLockOff));
                break;
        }
    }

    private Task ToggleRefreshRateAsync() => _refreshRateDispatcher.DispatchAsync(async () =>
    {
        try
        {
            if (!await _refreshRateFeature.IsSupportedAsync().ConfigureAwait(false))
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Switch refresh rate after Fn+R...");

            var all = await _refreshRateFeature.GetAllStatesAsync().ConfigureAwait(false);
            var current = await _refreshRateFeature.GetStateAsync().ConfigureAwait(false);
            var excluded = _settings.Store.ExcludedRefreshRates;

            var filtered = all.Except(excluded).ToArray();

            if (Log.Instance.IsTraceEnabled)
            {
                Log.Instance.Trace($"Refresh rates: [all={string.Join(", ", all.Select(r => r.Frequency))}]");
                Log.Instance.Trace($" - All: {string.Join(", ", all.Select(r => r.Frequency))}");
                Log.Instance.Trace($" - Excluded: {string.Join(", ", excluded.Select(r => r.Frequency))}");
                Log.Instance.Trace($" - Filtered: {string.Join(", ", filtered.Select(r => r.Frequency))}");
            }

            if (filtered.Length < 2)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Can't switch refresh rate after Fn+R when there is less than one available.");
                return;
            }

            var currentIndex = Array.IndexOf(filtered, current);
            var newIndex = currentIndex + 1;
            if (newIndex >= filtered.Length)
                newIndex = 0;

            var next = filtered[newIndex];

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Switching refresh rate after Fn+R to {next}...");

            await _refreshRateFeature.SetStateAsync(next).ConfigureAwait(false);

            _ = Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ =>
            {
                MessagingCenter.Publish(new Notification(NotificationType.RefreshRate, next.DisplayName));
            });

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Switched refresh rate after Fn+R to {next}.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to switch refresh rate after Fn+R.", ex);
        }
    });

    private static void OpenSnippingTool()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Starting snipping tool..");

        Process.Start("explorer", "ms-screenclip:");
    }

    private static void NotifySpectrumBacklight(SpectrumKeyboardBacklightBrightness value)
    {
        var type = value is SpectrumKeyboardBacklightBrightness.Off
            ? NotificationType.SpectrumBacklightOff
            : NotificationType.SpectrumBacklightChanged;
        MessagingCenter.Publish(new Notification(type, value));
    }

    private static void NotifySpectrumPreset(int value) => MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightPresetChanged, value));

    private async Task ToggleMicrophoneAsync()
    {

        if (!await _microphoneFeature.IsSupportedAsync().ConfigureAwait(false))
            return;

        switch (await _microphoneFeature.GetStateAsync().ConfigureAwait(false))
        {
            case MicrophoneState.On:
                await _microphoneFeature.SetStateAsync(MicrophoneState.Off).ConfigureAwait(false);
                MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOff));
                break;
            case MicrophoneState.Off:
                await _microphoneFeature.SetStateAsync(MicrophoneState.On).ConfigureAwait(false);
                MessagingCenter.Publish(new Notification(NotificationType.MicrophoneOn));
                break;
        }
    }

    private static void OpenAirplaneModeSettings() => AirplaneMode.Open();

    private static void NotifyWhiteBacklight(WhiteKeyboardBacklightState value)
    {
        var type = value is WhiteKeyboardBacklightState.Off
            ? NotificationType.WhiteKeyboardBacklightOff
            : NotificationType.WhiteKeyboardBacklightChanged;
        MessagingCenter.Publish(new Notification(type, value));
    }
}
