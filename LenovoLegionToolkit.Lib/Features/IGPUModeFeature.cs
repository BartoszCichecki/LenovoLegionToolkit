using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class IGPUModeFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
    {
        public IGPUModeFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode") { }

        public IntPtr Hook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg != NativeConstants.WM_DEVICECHANGE || lparam == IntPtr.Zero)
                return IntPtr.Zero;

            var devBroadcastHdr = Marshal.PtrToStructure<DevBroadcastHdr>(lparam);
            if (devBroadcastHdr.DeviceType != NativeConstants.DBT_DEVTYP_HANDLE)
                return IntPtr.Zero;

            var devBroadcastDeviceInterface = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(lparam);
            if (devBroadcastDeviceInterface.ClassGuid != NativeConstants.GUID_DISPLAY_DEVICE_ARRIVAL)
                return IntPtr.Zero;

            Task.Run(NotifyDGPUStatus);
            handled = true;

            return IntPtr.Zero;
        }

        private async Task NotifyDGPUStatus()
        {
            try
            {
                if (!await IsSupportedAsync().ConfigureAwait(false))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Unsupported.");
                    return;
                }

                var state = await GetStateAsync().ConfigureAwait(false);
                await WMI.CallAsync(Scope,
                    Query,
                    "NotifyDGPUStatus",
                    new() { { "Status", ToInternal(state).ToString() } }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error occurred.", ex);

            }
        }
    }
}