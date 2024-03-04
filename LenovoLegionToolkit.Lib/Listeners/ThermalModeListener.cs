using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class ThermalModeListener : AbstractWMIListener<ThermalModeListener.ChangedEventArgs, ThermalModeState, int>
{
    public class ChangedEventArgs : EventArgs
    {
        public ThermalModeState State { get; init; }
    }

    private readonly ThreadSafeCounter _suppressCounter = new();

    private readonly PowerPlanController _powerPlanController;

    public ThermalModeListener(PowerPlanController powerPlanController) : base(WMI.LenovoGameZoneThermalModeEvent.Listen)
    {
        _powerPlanController = powerPlanController ?? throw new ArgumentNullException(nameof(powerPlanController));
    }

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

    protected override ChangedEventArgs GetEventArgs(ThermalModeState value) => new() { State = value };

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
