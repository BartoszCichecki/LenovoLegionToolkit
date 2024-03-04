using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class DisplayBrightnessListener(PowerPlanController powerPlanController, ApplicationSettings settings)
    : AbstractWMIListener<DisplayBrightnessListener.ChangedEventArgs, Brightness, byte>(WMI.WmiMonitorBrightnessEvent
        .Listen)
{
    public class ChangedEventArgs(Brightness brightness) : EventArgs
    {
        public Brightness Brightness { get; } = brightness;
    }

    private const string DISPLAY_SUBGROUP_GUID = "7516b95f-f776-4464-8c53-06167f40cc99";
    private const string DISPLAY_BRIGHTNESS_SETTING_GUID = "aded5e82-b909-4619-9949-f5d71dac0bcb";

    private readonly ThrottleLastDispatcher _dispatcher = new(TimeSpan.FromSeconds(2), nameof(DisplayBrightnessListener));

    protected override Brightness GetValue(byte value) => new(value);

    protected override ChangedEventArgs GetEventArgs(Brightness value) => new(value);

    protected override async Task OnChangedAsync(Brightness value)
    {
        await SynchronizeBrightnessAsync(value).ConfigureAwait(false);
    }

    private async Task SynchronizeBrightnessAsync(Brightness value)
    {
        if (!settings.Store.SynchronizeBrightnessToAllPowerPlans)
            return;

        await _dispatcher.DispatchAsync(() => SetBrightnessForAllPowerPlansAsync(value)).ConfigureAwait(false);
    }

    private async Task SetBrightnessForAllPowerPlansAsync(Brightness brightness)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting brightness to {brightness.Value}...");

            var powerPlans = powerPlanController.GetPowerPlans();

            foreach (var powerPlan in powerPlans)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Modifying power plan {powerPlan.Name}... [powerPlan.Guid={powerPlan.Guid}, brightness={brightness.Value}]");

                await SetBrightnessForPowerPlansAsync(powerPlan, brightness, true).ConfigureAwait(false);
                await SetBrightnessForPowerPlansAsync(powerPlan, brightness, false).ConfigureAwait(false);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Brightness set to {brightness.Value}.");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to set brightness to {brightness.Value}.", ex);
        }
    }

    private static async Task SetBrightnessForPowerPlansAsync(PowerPlan powerPlan, Brightness brightness, bool isAc)
    {
        var option = isAc ? "/SETACVALUEINDEX" : "/SETDCVALUEINDEX";
        await CMD.RunAsync("powercfg", $"{option} {powerPlan.Guid} {DISPLAY_SUBGROUP_GUID} {DISPLAY_BRIGHTNESS_SETTING_GUID} {brightness.Value}").ConfigureAwait(false);
    }
}
