using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.System
{
    public static class Microphone
    {
        private static readonly MMDeviceEnumerator _enumerator = new();

        private static MMDeviceCollection Devices => _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

        public static bool IsEnabled
        {
            get
            {
                var enabled = true;
                foreach (var device in Devices)
                    enabled &= !device.AudioEndpointVolume.Mute;
                return enabled;
            }
        }

        public static void Toggle()
        {
            if (IsEnabled)
                Disable();
            else
                Enable();
        }

        public static void Enable()
        {
            foreach (var device in Devices)
                device.AudioEndpointVolume.Mute = false;
        }

        public static void Disable()
        {
            foreach (var device in Devices)
                device.AudioEndpointVolume.Mute = true;
        }
    }
}
