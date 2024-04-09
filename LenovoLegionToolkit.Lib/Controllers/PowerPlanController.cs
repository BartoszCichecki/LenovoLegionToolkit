﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;

namespace LenovoLegionToolkit.Lib.Controllers;

public class PowerPlanController(ApplicationSettings settings, VantageDisabler vantageDisabler)
{
    private static readonly Guid DefaultPowerPlan = Guid.Parse("381b4222-f694-41f0-9685-ff5bb260df2e");

    public IEnumerable<PowerPlan> GetPowerPlans(bool includeOverlays = false)
    {
        var powerPlanGuids = GetPowerPlanGuids(false);
        var activePowerPlanGuid = GetActivePowerPlanGuid();

        foreach (var powerPlanGuid in powerPlanGuids)
        {
            var powerPlaneName = GetPowerPlanName(powerPlanGuid);
            yield return new PowerPlan(powerPlanGuid, powerPlaneName, powerPlanGuid == activePowerPlanGuid, false);
        }

        if (!includeOverlays)
            yield break;

        var overlayGuids = GetPowerPlanGuids(true);

        foreach (var powerPlanGuid in overlayGuids)
        {
            var powerPlaneName = GetPowerPlanName(powerPlanGuid);
            yield return new PowerPlan(powerPlanGuid, powerPlaneName, powerPlanGuid == activePowerPlanGuid, true);
        }
    }

    public async Task ActivatePowerPlanAsync(PowerModeState powerModeState, bool alwaysActivateDefaults = false)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Activating... [powerModeState={powerModeState}, alwaysActivateDefaults={alwaysActivateDefaults}]");

        var powerPlanId = settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
        var isDefault = false;

        if (powerPlanId == Guid.Empty)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan for power mode {powerModeState} was not found in settings");

            powerPlanId = DefaultPowerPlan;
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

    public void Set(PowerPlan powerPlan, Brightness brightness)
    {
        PInvoke.PowerWriteACValueIndex(NullSafeHandle.Null, powerPlan.Guid, PInvoke.GUID_VIDEO_SUBGROUP, PInvokeExtensions.DISPLAY_BRIGTHNESS_SETTING_GUID, brightness.Value);
        PInvoke.PowerWriteDCValueIndex(NullSafeHandle.Null, powerPlan.Guid, PInvoke.GUID_VIDEO_SUBGROUP, PInvokeExtensions.DISPLAY_BRIGTHNESS_SETTING_GUID, brightness.Value);
    }

    private async Task<bool> ShouldActivateAsync(bool alwaysActivateDefaults, bool isDefault)
    {
        if (isDefault && alwaysActivateDefaults)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Power plan is default and always active defaults is set");

            return true;
        }

        var status = await vantageDisabler.GetStatusAsync().ConfigureAwait(false);
        if (status is SoftwareStatus.NotFound or SoftwareStatus.Disabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Vantage is active [status={status}]");

            return true;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Criteria for activation not met [isDefault={isDefault}, alwaysActivateDefaults={alwaysActivateDefaults}, status={status}]");

        return false;
    }

    private static unsafe List<Guid> GetPowerPlanGuids(bool overlay)
    {
        var list = new List<Guid>();

        var bufferSize = (uint)Marshal.SizeOf<Guid>();
        var buffer = new byte[bufferSize];

        var flags = overlay ? POWER_DATA_ACCESSOR.ACCESS_OVERLAY_SCHEME : POWER_DATA_ACCESSOR.ACCESS_SCHEME;

        fixed (byte* bufferPtr = buffer)
        {
            uint index = 0;
            while (PInvoke.PowerEnumerate(null, null, null, flags, index, bufferPtr, ref bufferSize) == WIN32_ERROR.ERROR_SUCCESS)
            {
                list.Add(new Guid(buffer));
                index++;
            }
        }

        return list;
    }

    private static unsafe string GetPowerPlanName(Guid powerPlanGuid)
    {
        var nameSize = 2048u;
        var namePtr = Marshal.AllocHGlobal((int)nameSize);

        try
        {
            if (PInvoke.PowerReadFriendlyName(null, powerPlanGuid, null, null, (byte*)namePtr.ToPointer(), ref nameSize) != WIN32_ERROR.ERROR_SUCCESS)
                PInvokeExtensions.ThrowIfWin32Error("PowerReadFriendlyName");

            return Marshal.PtrToStringUni(namePtr) ?? string.Empty;
        }
        catch
        {
            return powerPlanGuid.ToString();
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
