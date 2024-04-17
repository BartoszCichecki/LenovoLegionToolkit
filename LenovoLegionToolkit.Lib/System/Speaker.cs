using System.Linq;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.System;

public class Speaker
{
    public static void Mute() => ToggleMute(true);

    public static void Unmute() => ToggleMute(false);

    private static void ToggleMute(bool setMute)
    {
        var enumerator = new MMDeviceEnumerator();
        MMDevice mMDevice = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                                      .ToArray()
                                      .ToList()[0];
        mMDevice.AudioEndpointVolume.Mute = setMute;
    }
}
