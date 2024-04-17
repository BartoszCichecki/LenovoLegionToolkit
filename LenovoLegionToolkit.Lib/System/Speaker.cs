using System;
using System.Linq;
using LenovoLegionToolkit.Lib.Utils;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.System;

public class Speaker
{
    public static void Mute() => ToggleMute(true);

    public static void Unmute() => ToggleMute(false);

    private static void ToggleMute(bool setMute)
    {
        var enumerator = new MMDeviceEnumerator();
        try
        {
            MMDevice mMDevice = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                                          .ToArray()[0];
            mMDevice.AudioEndpointVolume.Mute = setMute;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to toggle mute [setMute={setMute}]", ex);
        }
    }
}
