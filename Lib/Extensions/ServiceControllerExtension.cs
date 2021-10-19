using LenovoLegionToolkit.Lib.Utils;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.ServiceProcess
{
    public static class ServiceControllerExtension
    {
        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

        public static void ChangeStartMode(this ServiceController svc, ServiceStartMode mode)
        {
            var scManagerHandle = IntPtr.Zero;
            var serviceHandle = IntPtr.Zero;

            try
            {
                scManagerHandle = Native.OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (scManagerHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Manager Error");

                serviceHandle = Native.OpenService(
                    scManagerHandle,
                    svc.ServiceName,
                    SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

                if (serviceHandle == IntPtr.Zero)
                    throw new ExternalException("Open Service Error");

                var result = Native.ChangeServiceConfig(
                    serviceHandle,
                    SERVICE_NO_CHANGE,
                    (uint)mode,
                    SERVICE_NO_CHANGE,
                    null,
                    null,
                    IntPtr.Zero,
                    null,
                    null,
                    null,
                    null);

                if (result == false)
                {
                    var nError = Marshal.GetLastWin32Error();
                    var win32Exception = new Win32Exception(nError);
                    throw new ExternalException($"Could not change service start type: {win32Exception.Message}");
                }

            }
            finally
            {
                if (serviceHandle != IntPtr.Zero)
                    _ = Native.CloseServiceHandle(serviceHandle);
                if (scManagerHandle != IntPtr.Zero)
                    _ = Native.CloseServiceHandle(scManagerHandle);
            }
        }
    }
}