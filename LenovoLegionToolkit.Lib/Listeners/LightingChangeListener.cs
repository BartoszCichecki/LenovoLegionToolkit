using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class LightingChangeListener : AbstractWMIListener<LightingChangeState>
{
    private readonly FnKeys _fnKeys;

    public LightingChangeListener(FnKeys fnKeys) : base("ROOT\\WMI", "LENOVO_LIGHTING_EVENT")
    {
        _fnKeys = fnKeys;
    }

    protected override LightingChangeState GetValue(PropertyDataCollection properties)
    {
        var property = properties["Key_ID"];
        var propertyValue = Convert.ToInt32(property.Value);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Event received. [value={propertyValue}]");

        var result = (LightingChangeState)propertyValue;
        return result;
    }

    protected override async Task OnChangedAsync(LightingChangeState value)
    {
        if (await _fnKeys.GetStatusAsync().ConfigureAwait(false) == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ignoring, FnKeys are enabled.");

            return;
        }

        if (value == LightingChangeState.Ports)
        {
            if (await IsPortLightingOnAsync().ConfigureAwait(false))
                MessagingCenter.Publish(new Notification(NotificationType.PortLightingOn, NotificationDuration.Short));
            else
                MessagingCenter.Publish(new Notification(NotificationType.PortLightingOff, NotificationDuration.Short));
        }

        if (value == LightingChangeState.Panel)
        {
            if (await IsPanelLogoLightingOnAsync().ConfigureAwait(false))
                MessagingCenter.Publish(new Notification(NotificationType.PanelLogoLightingOn, NotificationDuration.Short));
            else
                MessagingCenter.Publish(new Notification(NotificationType.PanelLogoLightingOff, NotificationDuration.Short));
        }
    }

    private Task<bool> IsPortLightingOnAsync() => IsLightingOnAsync(5);

    private Task<bool> IsPanelLogoLightingOnAsync() => IsLightingOnAsync(3);

    private async Task<bool> IsLightingOnAsync(int id) => await WMI.CallAsync("ROOT\\WMI",
        $"SELECT * FROM LENOVO_LIGHTING_METHOD",
        "Get_Lighting_Current_Status",
        new() { { "Lighting_ID", id } },
        pdc =>
        {
            if (!int.TryParse(pdc["Current_State_Type"].Value.ToString(), out var value))
                return false;
            return value == 1;
        });
}