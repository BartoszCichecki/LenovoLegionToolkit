using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners;

public class PowerModeListener : AbstractWMIListener<PowerModeState>, INotifyingListener<PowerModeState>
{
    private readonly AIModeController _aiModeController;

    public PowerModeListener(AIModeController aiModeController) : base("ROOT\\WMI", "LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT")
    {
        _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
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
}