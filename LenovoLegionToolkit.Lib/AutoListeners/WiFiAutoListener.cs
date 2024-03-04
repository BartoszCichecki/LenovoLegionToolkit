using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.NetworkManagement.WiFi;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public class WiFiAutoListener : AbstractAutoListener<WiFiAutoListener.ChangedEventArgs>
{
    public class ChangedEventArgs(bool isConnected, string? ssid) : EventArgs
    {
        public bool IsConnected { get; } = isConnected;
        public string? Ssid { get; } = ssid;
    }

    private readonly IMainThreadDispatcher _mainThreadDispatcher;
    private readonly WLAN_NOTIFICATION_CALLBACK _wlanCallback;

    private LambdaDisposable? _wlanNotificationDisposable;

    public unsafe WiFiAutoListener(IMainThreadDispatcher mainThreadDispatcher)
    {
        _mainThreadDispatcher = mainThreadDispatcher;

        _wlanCallback = WlanCallback;
    }

    protected override Task StartAsync() => _mainThreadDispatcher.DispatchAsync(() =>
    {
        _wlanNotificationDisposable = RegisterWlanNotification();
        return Task.CompletedTask;
    });

    protected override Task StopAsync() => _mainThreadDispatcher.DispatchAsync(() =>
    {
        _wlanNotificationDisposable?.Dispose();
        _wlanNotificationDisposable = null;
        return Task.CompletedTask;
    });

    private unsafe LambdaDisposable? RegisterWlanNotification()
    {
        var handlePtr = IntPtr.Zero;

        try
        {
            handlePtr = Marshal.AllocHGlobal(Marshal.SizeOf<HANDLE>());

            if (PInvoke.WlanOpenHandle(2, out _, (HANDLE*)handlePtr) != 0)
                return null;

            var handle = Marshal.PtrToStructure<HANDLE>(handlePtr);

            var result = PInvoke.WlanRegisterNotification(handle,
                WLAN_NOTIFICATION_SOURCES.WLAN_NOTIFICATION_SOURCE_ACM,
                true,
                _wlanCallback,
                null,
                null,
                null);
            if (result != 0)
                return null;

            return new LambdaDisposable(() =>
            {
                _ = PInvoke.WlanRegisterNotification(handle,
                    WLAN_NOTIFICATION_SOURCES.WLAN_NOTIFICATION_SOURCE_NONE,
                    true,
                    _wlanCallback,
                    null,
                    null,
                    null);

                _ = PInvoke.WlanCloseHandle(handle, null);
            });
        }
        finally
        {
            Marshal.FreeHGlobal(handlePtr);
        }
    }

    private unsafe void WlanCallback(L2_NOTIFICATION_DATA* param0, void* param1)
    {
        var data = Marshal.PtrToStructure<L2_NOTIFICATION_DATA>(new(param0));

        switch (data.NotificationCode)
        {
            case 0x0A: /* Connected */
                var ssid = GetSsid(data);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WiFi connected. [ssid={ssid}]");

                RaiseChanged(new ChangedEventArgs(true, ssid));
                break;
            case 0x15: /* Disconnected */
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"WiFi disconnected.");

                RaiseChanged(new ChangedEventArgs(false, null));
                break;
        }
    }

    private static unsafe string GetSsid(L2_NOTIFICATION_DATA data)
    {
        var notificationData = Marshal.PtrToStructure<WLAN_CONNECTION_NOTIFICATION_DATA>(new(data.pData));
        var dot11Ssid = notificationData.dot11Ssid;
        return Encoding.UTF8.GetString(dot11Ssid.ucSSID.Value, (int)dot11Ssid.uSSIDLength);
    }
}
