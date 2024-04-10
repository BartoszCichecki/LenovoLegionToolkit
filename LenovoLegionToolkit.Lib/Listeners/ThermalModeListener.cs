using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class ThermalModeListener(PowerPlanController powerPlanController)
    : AbstractWMIListener<ThermalModeListener.ChangedEventArgs, ThermalModeState, int>(
        WMI.LenovoGameZoneThermalModeEvent.Listen)
{
    public class ChangedEventArgs(ThermalModeState state) : EventArgs
    {
        public ThermalModeState State { get; } = state;
    }

    private readonly ThreadSafeCounter _suppressCounter = new();

    protected override ThermalModeState GetValue(int value)
    {
        var state = (ThermalModeState)value;

        if (!Enum.IsDefined(state))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Unknown value received: {value}");

            state = ThermalModeState.Unknown;
        }

        return state;
    }

    protected override ChangedEventArgs GetEventArgs(ThermalModeState value) => new(value);

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
                await powerPlanController.SetPowerPlanAsync(PowerModeState.Quiet).ConfigureAwait(false);
                break;
            case ThermalModeState.Balance:
                await powerPlanController.SetPowerPlanAsync(PowerModeState.Balance).ConfigureAwait(false);
                break;
            case ThermalModeState.Performance:
                await powerPlanController.SetPowerPlanAsync(PowerModeState.Performance).ConfigureAwait(false);
                break;
            case ThermalModeState.GodMode:
                await powerPlanController.SetPowerPlanAsync(PowerModeState.GodMode).ConfigureAwait(false);
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
