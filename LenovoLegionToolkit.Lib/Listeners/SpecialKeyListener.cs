using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
    {
        private readonly ThrottleFirstDispatcher _refreshRateDispatcher = new(TimeSpan.FromSeconds(1.5));

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
            SpecialKey.Fn_LockOn or SpecialKey.Fn_LockOff => NotifyFnLockState(value),
            SpecialKey.Fn_R or SpecialKey.Fn_R_2 => ToggleRefreshRateAsync(),
            SpecialKey.Fn_PrtSc => OpenSnippingTool(),
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

                if (value == SpecialKey.Fn_LockOn)
                    MessagingCenter.Publish(new Notification(NotificationType.FnLockOn, NotificationDuration.Short));

                if (value == SpecialKey.Fn_LockOff)
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

                all = all.Except(_settings.Store.ExcludedRefreshRates).ToArray();

                if (all.Length < 2)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Can't switch refresh rate after Fn+R. [all={all?.Length}]");

                    return;
                }

                var currentIndex = Array.IndexOf(all, current);
                var newIndex = currentIndex + 1;
                if (newIndex >= all.Length)
                    newIndex = 0;

                var next = all[newIndex];

                await _refreshRateFeature.SetStateAsync(next).ConfigureAwait(false);

                _ = Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
                {
                    MessagingCenter.Publish(new Notification(NotificationType.RefreshRate, NotificationDuration.Long, next.DisplayName));
                });

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Switched refresh rate after Fn+R to {next}.");
            }
            catch
            {
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

                if (value == 0)
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightOff, NotificationDuration.Short));
                else
                    MessagingCenter.Publish(new Notification(NotificationType.SpectrumBacklightOn, NotificationDuration.Short, value));
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
}
