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
        private readonly ApplicationSettings _settings;
        private readonly FnKeys _fnKeys;
        private readonly RefreshRateFeature _feature;

        public SpecialKeyListener(ApplicationSettings settings, FnKeys fnKeys, RefreshRateFeature feature) : base("ROOT\\WMI", "LENOVO_UTILITY_EVENT")
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _fnKeys = fnKeys ?? throw new ArgumentNullException(nameof(fnKeys));
            _feature = feature ?? throw new ArgumentNullException(nameof(feature));
        }

        protected override SpecialKey GetValue(PropertyDataCollection properties)
        {
            var property = properties["PressTypeDataVal"];
            var propertyValue = Convert.ToInt32(property.Value);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received: [value={propertyValue}]");

            var value = (SpecialKey)(object)propertyValue;
            return value;
        }

        protected override Task OnChangedAsync(SpecialKey value) => value switch
        {
            SpecialKey.Fn_R or SpecialKey.Fn_R_2 => ToggleRefreshRateAsync(),
            SpecialKey.Fn_PrtSc => OpenSnippingTool(),
            _ => Task.CompletedTask
        };

        private async Task ToggleRefreshRateAsync()
        {
            try
            {
                if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Ignoring Fn+R, FnKeys are enabled.");

                    return;
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Switch refresh rate after Fn+R...");

                var all = await _feature.GetAllStatesAsync().ConfigureAwait(false);
                var current = await _feature.GetStateAsync().ConfigureAwait(false);

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

                await _feature.SetStateAsync(next).ConfigureAwait(false);

                MessagingCenter.Publish(new Notification(NotificationIcon.RefreshRate, next.DisplayName, NotificationDuration.Long));

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Switched refresh rate after Fn+R to {next}.");
            }
            catch { }
        }

        private async Task OpenSnippingTool()
        {
            if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring Fn+PrtSc, FnKeys are enabled.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting snipping tool..");

            try
            {
                Process.Start("snippingtool");
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to start snipping tool.", ex);
            }
        }
    }
}
