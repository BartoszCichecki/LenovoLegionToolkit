using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.NetworkManagement.WiFi;

namespace LenovoLegionToolkit.Lib.System;

public static class WiFi
{
    public static unsafe string? GetConnectedNetworkSSID()
    {
        var handlePtr = IntPtr.Zero;
        var handle = HANDLE.Null;

        try
        {
            handlePtr = Marshal.AllocHGlobal(Marshal.SizeOf<HANDLE>());

            if (PInvoke.WlanOpenHandle(2, out _, (HANDLE*)handlePtr) != 0)
                return null;

            handle = Marshal.PtrToStructure<HANDLE>(handlePtr);

            WLAN_INTERFACE_INFO_LIST* ppInterfaceList;
            if (PInvoke.WlanEnumInterfaces(handle, null, &ppInterfaceList) != 0)
                return null;

            var interfaceGuid = ppInterfaceList->InterfaceInfo._0.InterfaceGuid;

            uint dataSize;
            void* data;

            if (PInvoke.WlanQueryInterface(handle, &interfaceGuid, WLAN_INTF_OPCODE.wlan_intf_opcode_interface_state, null, &dataSize, &data, null) != 0)
                return null;

            var state = *(WLAN_INTERFACE_STATE*)data;
            if (state != WLAN_INTERFACE_STATE.wlan_interface_state_connected)
                return null;

            if (PInvoke.WlanQueryInterface(handle, &interfaceGuid, WLAN_INTF_OPCODE.wlan_intf_opcode_current_connection, null, &dataSize, &data, null) != 0)
                return null;

            var attributes = *(WLAN_CONNECTION_ATTRIBUTES*)data;
            var dot11Ssid = attributes.wlanAssociationAttributes.dot11Ssid;
            var ssid = Encoding.UTF8.GetString(dot11Ssid.ucSSID.Value, (int)dot11Ssid.uSSIDLength);

            return ssid;
        }
        finally
        {
            _ = PInvoke.WlanCloseHandle(handle);
            Marshal.FreeHGlobal(handlePtr);
        }
    }
}
