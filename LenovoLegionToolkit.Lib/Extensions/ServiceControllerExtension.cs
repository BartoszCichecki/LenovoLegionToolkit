using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.System.Services;

namespace LenovoLegionToolkit.Lib.Extensions
{
    internal static class ServiceControllerExtension
    {
        public static unsafe void ChangeStartMode(this ServiceController svc, bool enabled)
        {
            var scManagerHandle = new SC_HANDLE();
            var serviceHandle = new SC_HANDLE();

            try
            {
                scManagerHandle = PInvoke.OpenSCManager(null as string, null, PInvoke.SC_MANAGER_ALL_ACCESS);
                if (scManagerHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Manager Error");

                serviceHandle = PInvoke.OpenService(scManagerHandle, svc.ServiceName, PInvoke.SERVICE_CHANGE_CONFIG);
                if (serviceHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Error");

                var result = PInvoke.ChangeServiceConfig(serviceHandle,
                    PInvoke.SERVICE_NO_CHANGE,
                    enabled ? SERVICE_START_TYPE.SERVICE_AUTO_START : SERVICE_START_TYPE.SERVICE_DISABLED,
                    SERVICE_ERROR.SERVICE_ERROR_NORMAL,
                    null as string,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

                if (result)
                    return;

                PInvokeExtensions.ThrowIfWin32Error($"Could not change service: {svc.ServiceName}");
            }
            finally
            {
                PInvoke.CloseServiceHandle(serviceHandle);
                PInvoke.CloseServiceHandle(scManagerHandle);
            }
        }
    }
}