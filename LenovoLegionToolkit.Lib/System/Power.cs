using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;

namespace LenovoLegionToolkit.Lib.System;

public static class Power
{
    public static async Task<PowerAdapterStatus> IsPowerAdapterConnectedAsync()
    {
        if (!PInvoke.GetSystemPowerStatus(out var sps))
            return PowerAdapterStatus.Connected;

        var adapterConnected = sps.ACLineStatus == 1;
        var acFitForOc = await IsAcFitForOc().ConfigureAwait(false) ?? true;
        var chargingNormally = await IsChargingNormally().ConfigureAwait(false) ?? true;

        return (adapterConnected, acFitForOc && chargingNormally) switch
        {
            (true, false) => PowerAdapterStatus.ConnectedLowWattage,
            (true, _) => PowerAdapterStatus.Connected,
            (false, _) => PowerAdapterStatus.Disconnected,
        };
    }

    public static async Task RestartAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Restarting...");

        await CMD.RunAsync("shutdown", "/r /t 0").ConfigureAwait(false);
    }

    private static async Task<bool?> IsAcFitForOc()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "IsACFitForOC",
                new(),
                pdc => (uint)pdc["Data"].Value).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Mode = {result}");

            return result == 1;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<bool?> IsChargingNormally()
    {
        try
        {
            var result = await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "GetPowerChargeMode",
                new(),
                pdc => (uint)pdc["Data"].Value).ConfigureAwait(false);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Mode = {result}");

            return result == 1;
        }
        catch
        {
            return null;
        }
    }
}