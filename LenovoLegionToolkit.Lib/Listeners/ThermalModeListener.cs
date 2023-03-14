using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners;

public class ThermalModeListener : AbstractWMIListener<ThermalModeState>
{
    public ThermalModeListener() : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT") { }

    protected override ThermalModeState GetValue(PropertyDataCollection properties)
    {
        var property = properties["mode"];
        var propertyValue = Convert.ToInt32(property.Value);
        var value = (ThermalModeState)(object)propertyValue;

        if (!Enum.IsDefined(value))
            value = ThermalModeState.Unknown;

        return value;
    }

    protected override async Task OnChangedAsync(ThermalModeState state)
    {
        if (state == ThermalModeState.Unknown)
            return;

        switch (state)
        {
            case ThermalModeState.Quiet:
                await Power.ActivatePowerPlanAsync(PowerModeState.Quiet).ConfigureAwait(false);
                break;
            case ThermalModeState.Balance:
                await Power.ActivatePowerPlanAsync(PowerModeState.Balance).ConfigureAwait(false);
                break;
            case ThermalModeState.Performance:
                await Power.ActivatePowerPlanAsync(PowerModeState.Performance).ConfigureAwait(false);
                break;
            case ThermalModeState.GodMode:
                await Power.ActivatePowerPlanAsync(PowerModeState.GodMode).ConfigureAwait(false);
                break;
        }
    }
}