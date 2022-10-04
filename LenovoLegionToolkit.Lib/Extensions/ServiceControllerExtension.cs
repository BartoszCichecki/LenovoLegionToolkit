using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Vanara.PInvoke;

namespace LenovoLegionToolkit.Lib.Extensions
{
    internal static class ServiceControllerExtension
    {
        public static void ChangeStartMode(this ServiceController svc, bool enabled)
        {
            var scManagerHandle = new AdvApi32.SafeSC_HANDLE(IntPtr.Zero);
            var serviceHandle = new AdvApi32.SafeSC_HANDLE(IntPtr.Zero);

            try
            {
                scManagerHandle = AdvApi32.OpenSCManager(null, null, AdvApi32.ScManagerAccessTypes.SC_MANAGER_ALL_ACCESS);
                if (scManagerHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Manager Error");

                serviceHandle = AdvApi32.OpenService(
                    scManagerHandle,
                    svc.ServiceName,
                    AdvApi32.ServiceAccessTypes.SERVICE_CHANGE_CONFIG | AdvApi32.ServiceAccessTypes.SERVICE_CHANGE_CONFIG);

                if (serviceHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Error");

                var result = AdvApi32.ChangeServiceConfig(
                    serviceHandle,
                    AdvApi32.ServiceTypes.SERVICE_NO_CHANGE,
                    enabled ? AdvApi32.ServiceStartType.SERVICE_AUTO_START : AdvApi32.ServiceStartType.SERVICE_DISABLED,
                    AdvApi32.ServiceErrorControlType.SERVICE_NO_CHANGE);

                if (result)
                    return;

                var nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException($"Could not change service start type: {win32Exception.Message}");

            }
            finally
            {
                if (serviceHandle != IntPtr.Zero)
                    AdvApi32.CloseServiceHandle(serviceHandle);
                if (scManagerHandle != IntPtr.Zero)
                    AdvApi32.CloseServiceHandle(scManagerHandle);
            }
        }
    }
}