using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.Lib.Listeners;

public class ThermalModeListener : AbstractWMIListener<ThermalModeState>
{
    private readonly PowerModeFeature _powerModeFeature;
    private readonly PowerModeListener _powerModeListener;

    public ThermalModeListener(PowerModeFeature powerModeFeature, PowerModeListener powerModeListener)
        : base("ROOT\\WMI", "LENOVO_GAMEZONE_THERMAL_MODE_EVENT")
    {
        _powerModeFeature = powerModeFeature;
        _powerModeListener = powerModeListener;
    }

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

        var powerModeState = await _powerModeFeature.GetStateAsync();
        await _powerModeListener.NotifyAsync(powerModeState);
    }
}