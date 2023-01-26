using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
{
    private readonly ThrottleFirstDispatcher _refreshRateDispatcher = new(TimeSpan.FromSeconds(2), "refreshRate");

    private readonly ApplicationSettings _settings;
    private readonly FnKeys _fnKeys;
    private readonly RefreshRateFeature _refreshRateFeature;

    public SpecialKeyListener(ApplicationSettings settings, FnKeys fnKeys, RefreshRateFeature feature) : base("ROOT\\WMI", "LENOVO_UTILITY_EVENT")
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fnKeys = fnKeys ?? throw new ArgumentNullException(nameof(fnKeys));
        _refreshRateFeature = feature ?? throw new ArgumentNullException(nameof(feature));
    }

    protected override SpecialKey GetValue(PropertyDataCollection properties)
    {
        var property = properties["PressTypeDataVal"];
        var propertyValue = Convert.ToInt32(property.Value);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [value={propertyValue}]");

        var value = (SpecialKey)propertyValue;
        return value;
    }

    protected override Task OnChangedAsync(SpecialKey value) => value switch
    {
        SpecialKey.CameraOn or SpecialKey.CameraOff => NotifyCameraState(value),
        SpecialKey.FnLockOn or SpecialKey.FnLockOff => NotifyFnLockState(value),
        SpecialKey.FnR or SpecialKey.FnR2 => ToggleRefreshRateAsync(),
        SpecialKey.FnPrtSc => OpenSnippingTool(),
        SpecialKey.PanelLogoLightingOn or SpecialKey.PanelLogoLightingOff => NotifyPanelLogoLighting(value),
        SpecialKey.SpectrumBacklightOff => NotifySpectrumBacklight(0),
        SpecialKey.SpectrumBacklight1 => NotifySpectrumBacklight(1),
        SpecialKey.SpectrumBacklight2 => NotifySpectrumBacklight(2),
        SpecialKey.SpectrumBacklight3 => NotifySpectrumBacklight(3),
        SpecialKey.SpectrumPreset1 => NotifySpectrumPreset(1),
        SpecialKey.SpectrumPreset2 => NotifySpectrumPreset(2),
        SpecialKey.SpectrumPreset3 => NotifySpectrumPreset(3),
        SpecialKey.SpectrumPreset4 => NotifySpectrumPreset(4),
        SpecialKey.SpectrumPreset5 => NotifySpectrumPreset(5),
        SpecialKey.SpectrumPreset6 => NotifySpectrumPreset(6),
        _ => Task.CompletedTask
    };

    private async Task NotifyCameraState(SpecialKey value)
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            if (value == SpecialKey.CameraOn)
                MessagingCenter.Publish(new Notification(NotificationType.CameraOn, NotificationDuration.Short));

            if (value == SpecialKey.CameraOff)
                MessagingCenter.Publish(new Notification(NotificationType.CameraOff, NotificationDuration.Short));
        }
        catch { }
    }

    private async Task NotifyFnLockState(SpecialKey value)
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            if (value == SpecialKey.FnLockOn)
                MessagingCenter.Publish(new Notification(NotificationType.FnLockOn, NotificationDuration.Short));

            if (value == SpecialKey.FnLockOff)
                MessagingCenter.Publish(new Notification(NotificationType.FnLockOff, NotificationDuration.Short));
        }
        catch { }
    }

    private Task ToggleRefreshRateAsync() => _refreshRateDispatcher.DispatchAsync(async () =>
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

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
                Log.Instance.Trace($"Refresh rates: [all={string.Join(",", all.Select(r => r.Frequency))}]");
                Log.Instance.Trace($" - All: {string.Join(",", all.Select(r => r.Frequency))}");
                Log.Instance.Trace($" - Excluded: {string.Join(",", excluded.Select(r => r.Frequency))}");
                Log.Instance.Trace($" - Filtered: {string.Join(",", filtered.Select(r => r.Frequency))}");
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

            _ = Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
            {
                MessagingCenter.Publish(new Notification(NotificationType.RefreshRate, NotificationDuration.Long, next.DisplayName));
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

    private async Task OpenSnippingTool()
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting snipping tool..");

            Process.Start("snippingtool");
        }
        catch { }
    }

    private async Task NotifyPanelLogoLighting(SpecialKey value)
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            if (value == SpecialKey.PanelLogoLightingOn)
                MessagingCenter.Publish(new Notification(NotificationType.PanelLogoLightingOn, NotificationDuration.Short));

            if (value == SpecialKey.PanelLogoLightingOff)
                MessagingCenter.Publish(new Notification(NotificationType.PanelLogoLightingOff, NotificationDuration.Short));
        }
        catch { }
    }

    private async Task NotifySpectrumBacklight(int value)
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            switch (value)
            {
                case 0:
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightOff, NotificationDuration.Short, SpectrumKeyboardBacklightBrightness.Off));
                    break;
                case 1:
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightChanged, NotificationDuration.Short, SpectrumKeyboardBacklightBrightness.Low));
                    break;
                case 2:
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightChanged, NotificationDuration.Short, SpectrumKeyboardBacklightBrightness.Medium));
                    break;
                case 3:
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightChanged, NotificationDuration.Short, SpectrumKeyboardBacklightBrightness.High));
                    break;
            }
        }
        catch { }
    }

    private async Task NotifySpectrumPreset(int value)
    {
        try
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

                return;
            }

            MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightPresetChanged, NotificationDuration.Short, value));
        }
        catch { }
    }
}