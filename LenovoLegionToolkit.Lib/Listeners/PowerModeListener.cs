using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Listeners;

public class PowerModeListener : AbstractWMIListener<PowerModeListener.ChangedEventArgs, PowerModeState, int>, INotifyingListener<PowerModeListener.ChangedEventArgs, PowerModeState>
{
    public class ChangedEventArgs : EventArgs
    {
        public PowerModeState State { get; init; }
    }

    private readonly PowerPlanController _powerPlanController;

    public PowerModeListener(PowerPlanController powerPlanController)
        : base(WMI.LenovoGameZoneSmartFanModeEvent.Listen)
    {
        _powerPlanController = powerPlanController ?? throw new ArgumentNullException(nameof(powerPlanController));
    }

    protected override PowerModeState GetValue(int value)
    {
        var result = (PowerModeState)(value - 1);
        return result;
    }

    protected override ChangedEventArgs GetEventArgs(PowerModeState value) => new() { State = value };

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
        await _powerPlanController.ActivatePowerPlanAsync(value).ConfigureAwait(false);
    }

    private static void PublishNotification(PowerModeState value)
    {
        switch (value)
        {
            case PowerModeState.Quiet:
                MessagingCenter.Publish(new Notification(NotificationType.PowerModeQuiet, value.GetDisplayName()));
                break;
            case PowerModeState.Balance:
                MessagingCenter.Publish(new Notification(NotificationType.PowerModeBalance, value.GetDisplayName()));
                break;
            case PowerModeState.Performance:
                MessagingCenter.Publish(new Notification(NotificationType.PowerModePerformance, value.GetDisplayName()));
                break;
            case PowerModeState.GodMode:
                MessagingCenter.Publish(new Notification(NotificationType.PowerModeGodMode, value.GetDisplayName()));
                break;
        }
    }
}
