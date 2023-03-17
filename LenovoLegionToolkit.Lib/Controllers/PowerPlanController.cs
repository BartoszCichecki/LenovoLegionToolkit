using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.Controllers;

public class PowerPlanController
{
    private static readonly Dictionary<PowerModeState, Guid> DefaultPowerModes = new()
    {
        { PowerModeState.Quiet , Guid.Parse("16edbccd-dee9-4ec4-ace5-2f0b5f2a8975")},
        { PowerModeState.Balance , Guid.Parse("85d583c5-cf2e-4197-80fd-3789a227a72c")},
        { PowerModeState.Performance , Guid.Parse("52521609-efc9-4268-b9ba-67dea73f18b2")},
        { PowerModeState.GodMode , Guid.Parse("85d583c5-cf2e-4197-80fd-3789a227a72c")},
    };

    private readonly ApplicationSettings _settings;

    private readonly Vantage _vantage;

    public PowerPlanController(ApplicationSettings settings, Vantage vantage)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _vantage = vantage ?? throw new ArgumentNullException(nameof(vantage));
    }

    public IEnumerable<PowerPlan> GetPowerPlans()
    {
        var powerPlansGuid = GetPowerPlansGuid();
        var activePowerPlanGuid = GetActivePowerPlanGuid();

        foreach (var powerPlanGuid in powerPlansGuid)
        {
            var powerPlaneName = GetPowerPlanName(powerPlanGuid);
            yield return new PowerPlan(powerPlanGuid, powerPlaneName, powerPlanGuid == activePowerPlanGuid);
        }
    }

    public async Task ActivatePowerPlanAsync(PowerModeState powerModeState, bool alwaysActivateDefaults = false)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Activating... [powerModeState={powerModeState}, alwaysActivateDefaults={alwaysActivateDefaults}]");

        var powerPlanId = _settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
        var isDefault = false;

        if (powerPlanId == Guid.Empty)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan for power mode {powerModeState} was not found in settings");

            if (DefaultPowerModes.TryGetValue(powerModeState, out var defaultPowerPlanId))
                powerPlanId = defaultPowerPlanId;
            else
                throw new InvalidOperationException("Unknown state");

            isDefault = true;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Power plan to be activated is {powerPlanId} [isDefault={isDefault}]");

        if (!await ShouldActivateAsync(alwaysActivateDefaults, isDefault).ConfigureAwait(false))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan {powerPlanId} will not be activated [isDefault={isDefault}]");

            return;
        }

        var powerPlans = GetPowerPlans().ToArray();

        if (Log.Instance.IsTraceEnabled)
        {
            Log.Instance.Trace($"Available power plans:");
            foreach (var powerPlan in powerPlans)
                Log.Instance.Trace($" - {powerPlan.Name} [guid={powerPlan.Guid}, isActive={powerPlan.IsActive}]");
        }

        var powerPlanToActivate = powerPlans.FirstOrDefault(pp => pp.Guid == powerPlanId);
        if (powerPlanToActivate.Equals(default(PowerPlan)))
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan {powerPlanId} was not found");
            return;
        }

        if (powerPlanToActivate.IsActive)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan {powerPlanToActivate.Guid} is already active. [name={powerPlanToActivate.Name}]");
            return;
        }

        SetActivePowerPlan(powerPlanToActivate.Guid);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Power plan {powerPlanToActivate.Guid} activated. [name={powerPlanToActivate.Name}]");
    }

    public PowerModeState[] GetMatchingPowerModes(Guid powerPlanGuid)
    {
        var powerModes = new Dictionary<PowerModeState, Guid>(DefaultPowerModes);

        foreach (var kv in _settings.Store.PowerPlans)
        {
            powerModes[kv.Key] = kv.Value;
        }

        return powerModes.Where(kv => kv.Value == powerPlanGuid)
            .Select(kv => kv.Key)
            .ToArray();
    }

    private async Task<bool> ShouldActivateAsync(bool alwaysActivateDefaults, bool isDefault)
    {
        var activateWhenVantageEnabled = _settings.Store.ActivatePowerProfilesWithVantageEnabled;
        if (activateWhenVantageEnabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Activate power profiles with Vantage is enabled");

            return true;
        }

        if (isDefault && alwaysActivateDefaults)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan is default and always active defaults is set");

            return true;
        }

        var status = await _vantage.GetStatusAsync().ConfigureAwait(false);
        if (status is SoftwareStatus.NotFound or SoftwareStatus.Disabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Vantage is active [status={status}]");

            return true;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Criteria for activation not met [activateWhenVantageEnabled={activateWhenVantageEnabled}, isDefault={isDefault}, alwaysActivateDefaults={alwaysActivateDefaults}, status={status}]");

        return false;
    }

    private static unsafe IEnumerable<Guid> GetPowerPlansGuid()
    {
        var list = new List<Guid>();

        var bufferSize = (uint)Marshal.SizeOf<Guid>();
        var buffer = new byte[bufferSize];

        fixed (byte* bufferPtr = buffer)
        {
            uint index = 0;
            while (PInvoke.PowerEnumerate(null, null, null, POWER_DATA_ACCESSOR.ACCESS_SCHEME, index, bufferPtr, ref bufferSize) == WIN32_ERROR.ERROR_SUCCESS)
            {
                list.Add(new Guid(buffer));
                index++;
            }
        }

        return list;
    }

    private static unsafe string GetPowerPlanName(Guid powerPlanGuid)
    {
        var nameSize = 1024u;
        var namePtr = Marshal.AllocHGlobal((int)nameSize);

        try
        {
            if (PInvoke.PowerReadFriendlyName(null, powerPlanGuid, null, null, (byte*)namePtr.ToPointer(), ref nameSize) != WIN32_ERROR.ERROR_SUCCESS)
                PInvokeExtensions.ThrowIfWin32Error("PowerReadFriendlyName");

            return Marshal.PtrToStringUni(namePtr) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(namePtr);
        }
    }

    private static unsafe Guid GetActivePowerPlanGuid()
    {
        if (PInvoke.PowerGetActiveScheme(null, out var guid) != WIN32_ERROR.ERROR_SUCCESS)
            PInvokeExtensions.ThrowIfWin32Error("PowerGetActiveScheme");

        return Marshal.PtrToStructure<Guid>(new IntPtr(guid));
    }

    private static void SetActivePowerPlan(Guid powerPlanGuid)
    {
        if (PInvoke.PowerSetActiveScheme(null, powerPlanGuid) != WIN32_ERROR.ERROR_SUCCESS)
            PInvokeExtensions.ThrowIfWin32Error("PowerSetActiveScheme");
    }
}