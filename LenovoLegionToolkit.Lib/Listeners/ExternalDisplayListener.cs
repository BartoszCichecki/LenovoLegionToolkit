using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Windows.Win32;


namespace LenovoLegionToolkit.Lib.Listeners;

public class ExternalDisplayListener : IListener<EventArgs>
{
    private bool _started;

    public event EventHandler<EventArgs>? OnConnected;
    public event EventHandler<EventArgs>? OnDisconnected;
    public event EventHandler<EventArgs>? Changed;

    public ExternalDisplayListener()
    {
        MessagingCenter.Subscribe<DeviceBroadcast>(this, Handler);
    }

    public Task StartAsync()
    {
        if (_started)
            return Task.CompletedTask;

        _started = true;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _started = false;

        return Task.CompletedTask;
    }

    private void Handler(DeviceBroadcast broadcast)
    {
        if (broadcast.Class != PInvoke.GUID_DISPLAY_DEVICE_ARRIVAL)
            return;

        if (broadcast.Type == PInvoke.DBT_DEVICEARRIVAL)
        {
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        if (broadcast.Type == PInvoke.DBT_DEVICEREMOVECOMPLETE)
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}