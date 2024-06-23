using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features;

public class PowerModeUnavailableWithoutACException(PowerModeState powerMode) : Exception
{
    public PowerModeState PowerMode { get; } = powerMode;
}

public class PowerModeFeature(
    GodModeController godModeController,
    WindowsPowerModeController windowsPowerModeController,
    WindowsPowerPlanController windowsPowerPlanController,
    ThermalModeListener thermalModeListener,
    PowerModeListener powerModeListener)
    : AbstractWmiFeature<PowerModeState>(WMI.LenovoGameZoneData.GetSmartFanModeAsync, WMI.LenovoGameZoneData.SetSmartFanModeAsync, WMI.LenovoGameZoneData.IsSupportSmartFanAsync, 1)
{
    public bool AllowAllPowerModesOnBattery { get; set; }

    public override async Task<PowerModeState[]> GetAllStatesAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        return mi.Properties.SupportsGodMode
            ? [PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance, PowerModeState.GodMode]
            : [PowerModeState.Quiet, PowerModeState.Balance, PowerModeState.Performance];
    }

    public override async Task SetStateAsync(PowerModeState state)
    {
        var allStates = await GetAllStatesAsync().ConfigureAwait(false);
        if (!allStates.Contains(state))
            throw new InvalidOperationException($"Unsupported power mode {state}");

        if (state is PowerModeState.Performance or PowerModeState.GodMode
            && !AllowAllPowerModesOnBattery
            && await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false) is PowerAdapterStatus.Disconnected)
            throw new PowerModeUnavailableWithoutACException(state);

        var currentState = await GetStateAsync().ConfigureAwait(false);

        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);

        if (mi.Properties.HasQuietToPerformanceModeSwitchingBug && currentState == PowerModeState.Quiet && state == PowerModeState.Performance)
        {
            thermalModeListener.SuppressNext();
            await base.SetStateAsync(PowerModeState.Balance).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
        }

        if (mi.Properties.HasGodModeToOtherModeSwitchingBug && currentState == PowerModeState.GodMode && state != PowerModeState.GodMode)
        {
            thermalModeListener.SuppressNext();

            switch (state)
            {
                case PowerModeState.Quiet:
                    await base.SetStateAsync(PowerModeState.Performance).ConfigureAwait(false);
                    break;
                case PowerModeState.Balance:
                    await base.SetStateAsync(PowerModeState.Quiet).ConfigureAwait(false);
                    break;
                case PowerModeState.Performance:
                    await base.SetStateAsync(PowerModeState.Balance).ConfigureAwait(false);
                    break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);

        }

        thermalModeListener.SuppressNext();
        await base.SetStateAsync(state).ConfigureAwait(false);

        await powerModeListener.NotifyAsync(state).ConfigureAwait(false);
    }

    public async Task EnsureCorrectWindowsPowerSettingsAreSetAsync()
    {
        var state = await GetStateAsync().ConfigureAwait(false);
        await windowsPowerModeController.SetPowerModeAsync(state).ConfigureAwait(false);
        await windowsPowerPlanController.SetPowerPlanAsync(state, true).ConfigureAwait(false);
    }

    public async Task EnsureGodModeStateIsAppliedAsync()
    {
        var state = await GetStateAsync().ConfigureAwait(false);
        if (state != PowerModeState.GodMode)
            return;

        await godModeController.ApplyStateAsync().ConfigureAwait(false);
    }
}
