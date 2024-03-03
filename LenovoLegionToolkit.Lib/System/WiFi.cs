using System.Linq;
using LenovoLegionToolkit.Lib.Extensions;
using ManagedNativeWifi;

namespace LenovoLegionToolkit.Lib.System;

public static class WiFi
{
    public static void TurnOn()
    {
        NativeWifi.EnumerateInterfaces()
            .ForEach(i => NativeWifi.TurnOnInterfaceRadio(i.Id));
    }

    public static void TurnOff()
    {
        NativeWifi.EnumerateInterfaces()
            .ForEach(i => NativeWifi.TurnOffInterfaceRadio(i.Id));
    }

    public static string? GetConnectedNetworkSsid()
    {
        return NativeWifi.EnumerateConnectedNetworkSsids()
            .Select(c => c.ToString())
            .FirstOrDefault();
    }
}
