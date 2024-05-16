using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.Lib.Controllers;

public partial class WindowsPowerModeController(ApplicationSettings settings, IMainThreadDispatcher mainThreadDispatcher)
{
    private const string POWER_SCHEMES_HIVE = "HKEY_LOCAL_MACHINE";
    private const string POWER_SCHEMES_SUBKEY = "SYSTEM\\CurrentControlSet\\Control\\Power\\User\\PowerSchemes";
    private const string ACTIVE_OVERLAY_AC_POWER_SCHEME_KEY = "ActiveOverlayAcPowerScheme";
    private const string ACTIVE_OVERLAY_DC_POWER_SCHEME_KEY = "ActiveOverlayDcPowerScheme";

    private static readonly Guid DefaultPowerPlan = Guid.Parse("381b4222-f694-41f0-9685-ff5bb260df2e");
    private static readonly Guid BestPowerEfficiency = Guid.Parse("961cc777-2547-4f9d-8174-7d86181b8a7a");
    private static readonly Guid BestPerformance = Guid.Parse("ded574b5-45a0-4f42-8737-46345c09c238");

    private readonly ThrottleLastDispatcher _dispatcher = new(TimeSpan.FromSeconds(2), nameof(WindowsPowerModeController));

    public async Task SetPowerModeAsync(PowerModeState powerModeState)
    {
        if (settings.Store.PowerModeMappingMode is not PowerModeMappingMode.WindowsPowerMode)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ignoring... [powerModeMappingMode={settings.Store.PowerModeMappingMode}]");

            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Activating... [powerModeState={powerModeState}]");

        var powerMode = settings.Store.PowerModes.GetValueOrDefault(powerModeState, WindowsPowerMode.Balanced);
        var powerModeGuid = GuidForWindowsPowerMode(powerMode);

        if (Power.IsBatterySaverEnabled())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Battery saver is on - will not set overlay scheme.");

            return;
        }

        await _dispatcher.DispatchAsync(() =>
        {
            ActivateDefaultPowerPlanIfNeeded();

            mainThreadDispatcher.Dispatch(() =>
            {
                var result = PowerSetActiveOverlayScheme(powerModeGuid);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Overlay scheme set. [result={result}]");
            });

            try
            {
                UpdateRegistry(powerModeGuid);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to update registry.", ex);
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Power mode {powerMode} activated... [powerModeState={powerModeState}, powerModeGuid={powerModeGuid}]");
    }

    private static void UpdateRegistry(Guid guid)
    {
        Registry.SetValue(POWER_SCHEMES_HIVE, POWER_SCHEMES_SUBKEY, ACTIVE_OVERLAY_AC_POWER_SCHEME_KEY, guid, true);
        Registry.SetValue(POWER_SCHEMES_HIVE, POWER_SCHEMES_SUBKEY, ACTIVE_OVERLAY_DC_POWER_SCHEME_KEY, guid, true);
    }

    private static Guid GuidForWindowsPowerMode(WindowsPowerMode windowsPowerMode) => windowsPowerMode switch
    {
        WindowsPowerMode.BestPowerEfficiency => BestPowerEfficiency,
        WindowsPowerMode.BestPerformance => BestPerformance,
        _ => Guid.Empty
    };

    private static unsafe void ActivateDefaultPowerPlanIfNeeded()
    {
        if (PInvoke.PowerGetActiveScheme(null, out var guid) != WIN32_ERROR.ERROR_SUCCESS)
            PInvokeExtensions.ThrowIfWin32Error("PowerGetActiveScheme");

        if (DefaultPowerPlan == *guid)
            return;

        if (PInvoke.PowerSetActiveScheme(null, DefaultPowerPlan) != WIN32_ERROR.ERROR_SUCCESS)
            PInvokeExtensions.ThrowIfWin32Error("PowerSetActiveScheme");

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Activated default power plan.");
    }

    [LibraryImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
    private static partial uint PowerSetActiveOverlayScheme(Guid guid);
}
