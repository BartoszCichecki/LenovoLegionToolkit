using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Listeners;

public class DisplayBrightnessListener : AbstractWMIListener<Brightness>
{
    private const string DISPLAY_SUBGROUP_GUID = "7516b95f-f776-4464-8c53-06167f40cc99";
    private const string DISPLAY_BRIGHTNESS_SETTING_GUID = "aded5e82-b909-4619-9949-f5d71dac0bcb";

    private readonly ApplicationSettings _settings;

    private readonly ThrottleLastDispatcher _dispatcher = new(TimeSpan.FromSeconds(2));

    public DisplayBrightnessListener(ApplicationSettings settings) : base("ROOT\\WMI", "WmiMonitorBrightnessEvent")
    {
        _settings = settings;
    }

    protected override Brightness GetValue(PropertyDataCollection properties) => Convert.ToByte(properties["Brightness"].Value);

    protected override async Task OnChangedAsync(Brightness value)
    {
        await SynchronizeBrightnessAsync(value).ConfigureAwait(false);
    }

    private async Task SynchronizeBrightnessAsync(Brightness value)
    {
        if (!_settings.Store.SynchronizeBrightnessToAllPowerPlans)
            return;

        await _dispatcher.DispatchAsync(() => SetBrightnessForAllPowerPlansAsync(value)).ConfigureAwait(false);
    }

    private static async Task SetBrightnessForAllPowerPlansAsync(Brightness brightness)
    {
        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Setting brightness to {brightness.Value}...");

            var powerPlans = await Power.GetPowerPlansAsync().ConfigureAwait(false);

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