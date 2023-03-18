using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class ThermalModeListener : AbstractWMIListener<ThermalModeState>
{
    private readonly ThreadSafeCounter _suppressCounter = new();

    private readonly PowerPlanController _powerPlanController;

    public ThermalModeListener(PowerPlanController powerPlanController) : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT")
    {
        _powerPlanController = powerPlanController ?? throw new ArgumentNullException(nameof(powerPlanController));
    }

    protected override ThermalModeState GetValue(PropertyDataCollection properties)
    {
        var property = properties["mode"];
        var propertyValue = Convert.ToInt32(property.Value);
        var value = (ThermalModeState)(object)propertyValue;

        if (!Enum.IsDefined(value))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown value received: {propertyValue}");

            value = ThermalModeState.Unknown;
        }

        return value;
    }

    protected override async Task OnChangedAsync(ThermalModeState state)
    {
        if (!_suppressCounter.Decrement())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Suppressed.");
            return;
        }

        if (state == ThermalModeState.Unknown)
            return;

        switch (state)
        {
            case ThermalModeState.Quiet:
                await _powerPlanController.ActivatePowerPlanAsync(PowerModeState.Quiet).ConfigureAwait(false);
                break;
            case ThermalModeState.Balance:
                await _powerPlanController.ActivatePowerPlanAsync(PowerModeState.Balance).ConfigureAwait(false);
                break;
            case ThermalModeState.Performance:
                await _powerPlanController.ActivatePowerPlanAsync(PowerModeState.Performance).ConfigureAwait(false);
                break;
            case ThermalModeState.GodMode:
                await _powerPlanController.ActivatePowerPlanAsync(PowerModeState.GodMode).ConfigureAwait(false);
                break;
        }
    }

    public void SuppressNext()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Suppressing next...");

        _suppressCounter.Increment();
    }
}