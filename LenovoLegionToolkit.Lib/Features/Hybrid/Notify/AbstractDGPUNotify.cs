using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public abstract partial class AbstractDGPUNotify : IDGPUNotify
{
    [GeneratedRegex("pci#ven_([0-9A-Fa-f]{4})|dev_([0-9A-Fa-f]{4})")]
    private static partial Regex HardwareIdRegex();

    private readonly object _lock = new();

    private CancellationTokenSource? _notifyLaterCancellationTokenSource;

    public event EventHandler<bool>? Notified;

    public abstract Task<bool> IsSupportedAsync();

    public async Task<bool> IsDGPUAvailableAsync()
    {
        try
        {
            var dgpuHardwareId = await GetDGPUHardwareIdAsync().ConfigureAwait(false);
            var isAvailable = IsDGPUAvailable(dgpuHardwareId);
            return isAvailable;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to notify.", ex);
            return false;
        }
    }

    public async Task NotifyAsync(bool publish = true)
    {
        lock (_lock)
        {
            _notifyLaterCancellationTokenSource?.Cancel();
            _notifyLaterCancellationTokenSource = null;
        }

        try
        {
            var dgpuHardwareId = await GetDGPUHardwareIdAsync().ConfigureAwait(false);
            var isAvailable = IsDGPUAvailable(dgpuHardwareId);
            await NotifyDGPUStatusAsync(isAvailable).ConfigureAwait(false);

            if (publish)
                Notified?.Invoke(this, isAvailable);

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Notified: {isAvailable}");
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to notify.", ex);
        }
    }

    public Task NotifyLaterIfNeededAsync()
    {
        CancellationToken token;

        lock (_lock)
        {
            _notifyLaterCancellationTokenSource?.Cancel();
            _notifyLaterCancellationTokenSource = new();

            token = _notifyLaterCancellationTokenSource.Token;
        }

        _ = Task.Delay(TimeSpan.FromSeconds(5), token)
            .ContinueWith(async t =>
            {
                if (!t.IsCompletedSuccessfully)
                    return;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Event not received, notifying anyway...");

                await NotifyAsync(false).ConfigureAwait(false);
            }, token);

        return Task.CompletedTask;
    }

    protected abstract Task NotifyDGPUStatusAsync(bool state);

    protected abstract Task<HardwareId> GetDGPUHardwareIdAsync();

    private unsafe bool IsDGPUAvailable(HardwareId dgpuHardwareId)
    {
        if (dgpuHardwareId == HardwareId.Empty)
            return false;

        var guidDisplayDeviceArrival = PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL;
        var deviceHandle = PInvoke.SetupDiGetClassDevs(guidDisplayDeviceArrival,
            null,
            HWND.Null,
            SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_DEVICEINTERFACE | SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PRESENT | SETUP_DI_GET_CLASS_DEVS_FLAGS.DIGCF_PROFILE);

        uint index = 0;
        while (true)
        {
            var currentIndex = index;
            index++;

            var deviceInfoData = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };
            var result1 = PInvoke.SetupDiEnumDeviceInfo(deviceHandle, currentIndex, ref deviceInfoData);
            if (!result1)
            {
                if (Marshal.GetLastWin32Error() == PInvokeExtensions.ERROR_NO_MORE_ITEMS)
                    break;

                PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInfo");
            }

            var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>() };
            var result2 = PInvoke.SetupDiEnumDeviceInterfaces(deviceHandle, null, guidDisplayDeviceArrival, currentIndex, ref deviceInterfaceData);
            if (!result2)
                PInvokeExtensions.ThrowIfWin32Error("SetupDiEnumDeviceInterfaces");

            var requiredSize = 0u;
            _ = PInvoke.SetupDiGetDeviceInterfaceDetail(deviceHandle, deviceInterfaceData, null, 0, &requiredSize, null);

            string devicePath;
            var output = IntPtr.Zero;
            try
            {
                output = Marshal.AllocHGlobal((int)requiredSize);
                var deviceDetailData = (SP_DEVICE_INTERFACE_DETAIL_DATA_W*)output.ToPointer();
                deviceDetailData->cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DETAIL_DATA_W>();

                var result3 = PInvoke.SetupDiGetDeviceInterfaceDetail(deviceHandle, deviceInterfaceData, deviceDetailData, requiredSize, null, null);
                if (!result3)
                    PInvokeExtensions.ThrowIfWin32Error("SetupDiGetDeviceInterfaceDetail");

                devicePath = new string(&deviceDetailData->DevicePath.e0);
            }
            finally
            {
                Marshal.FreeHGlobal(output);
            }

            if (!devicePath.Contains(guidDisplayDeviceArrival.ToString()))
                continue;

            if (dgpuHardwareId != HardwareIdFromDevicePath(devicePath))
                continue;

            if (PInvoke.CM_Get_DevNode_Status(out var status, out _, deviceInfoData.DevInst, 0) != 0)
                continue;

            if (status.HasFlag(CM_DEVNODE_STATUS_FLAGS.DN_HAS_PROBLEM))
                continue;

            return true;
        }

        return false;
    }

    private static HardwareId HardwareIdFromDevicePath(string devicePath)
    {
        try
        {
            var matches = HardwareIdRegex().Matches(devicePath);
            if (matches.Count != 2)
                return default;

            var vendor = matches[0].Groups[1].Value;
            var device = matches[1].Groups[2].Value;

            return new(vendor, device);
        }
        catch
        {
            return default;
        }
    }
}
