using System;
using System.Linq;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class MicrophoneFeature : IFeature<MicrophoneState>
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private MMDeviceCollection Devices => _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

    public Task<bool> IsSupportedAsync() => Task.FromResult(Devices.Any());

    public Task<MicrophoneState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<MicrophoneState>());

    public Task<MicrophoneState> GetStateAsync()
    {
        var mute = Devices.Aggregate(true, (current, device) => current && device.AudioEndpointVolume.Mute);
        var result = mute ? MicrophoneState.Off : MicrophoneState.On;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(MicrophoneState state)
    {
        var mute = MicrophoneState.Off == state;
        foreach (var device in Devices)
            device.AudioEndpointVolume.Mute = mute;
        return Task.CompletedTask;
    }
}
