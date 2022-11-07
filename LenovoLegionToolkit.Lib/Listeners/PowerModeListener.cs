using System;
using System.ComponentModel.DataAnnotations;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>, INotifyingListener<PowerModeState>
    {
        private readonly AIModeController _aiModeController;
        private readonly PowerModeFeature _powerModeFeature;
        private readonly GodModeController _godModeController;

        public PowerModeListener(AIModeController aiModeController, PowerModeFeature powerModeFeature, GodModeController godModeController)
            : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT")
        {
            _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
            _powerModeFeature = powerModeFeature ?? throw new ArgumentNullException(nameof(powerModeFeature));
            _godModeController = godModeController ?? throw new ArgumentNullException(nameof(godModeController));
        }

        protected override PowerModeState GetValue(PropertyDataCollection properties)
        {
            var property = properties["mode"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (PowerModeState)(object)(propertyValue - 1);
            return value;
        }

        protected override async Task OnChangedAsync(PowerModeState value)
        {
            await ChangeDependenciesAsync(value).ConfigureAwait(false);
            PublishNotification(value);
        }

        public async Task NotifyAsync(PowerModeState value)
        {
            await ChangeDependenciesAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }

        private async Task ChangeDependenciesAsync(PowerModeState value)
        {
            await _aiModeController.StopAsync(value).ConfigureAwait(false);
            await _aiModeController.StartAsync(value).ConfigureAwait(false);

            if (value == PowerModeState.GodMode)
                await _godModeController.ApplyStateAsync().ConfigureAwait(false);

            await Power.ActivatePowerPlanAsync(value).ConfigureAwait(false);
        }

        private static void PublishNotification(PowerModeState value)
        {
            switch (value)
            {
                case PowerModeState.Quiet:
                    MessagingCenter.Publish(new Notification(NotificationType.PowerModeQuiet, NotificationDuration.Short, value.GetDisplayName()));
                    break;
                case PowerModeState.Balance:
                    MessagingCenter.Publish(new Notification(NotificationType.PowerModeBalance, NotificationDuration.Short, value.GetDisplayName()));
                    break;
                case PowerModeState.Performance:
                    MessagingCenter.Publish(new Notification(NotificationType.PowerModePerformance, NotificationDuration.Short, value.GetDisplayName()));
                    break;
                case PowerModeState.GodMode:
                    MessagingCenter.Publish(new Notification(NotificationType.PowerModeGodMode, NotificationDuration.Short, value.GetDisplayName()));
                    break;
            }
        }

        protected override async void Handler(PropertyDataCollection properties)
        {
            var value = GetValue(properties);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Event received. [value={value}, listener={GetType().Name}]");

            // Seems we can't trust the mode received from LENOVO_GAMEZONE_THERMAL_MODE_EVENT,
            // but at least it does fire reliably and we can call GetThermalMode ourselves.
            value = await _powerModeFeature.GetActualStateAsync().ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Patched the event. [value={value}, listener={GetType().Name}]");

            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }
    }
}
