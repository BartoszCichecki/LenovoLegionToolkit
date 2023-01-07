using System;
using System.Linq;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class MicrophoneMuteFeature : IFeature<MicrophoneMuteState>
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private MMDeviceCollection Devices => _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

    public Task<bool> IsSupportedAsync() => Task.FromResult(Devices.Any());

    public Task<MicrophoneMuteState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<MicrophoneMuteState>());

    public Task<MicrophoneMuteState> GetStateAsync()
    {
        var enabled = Devices.Aggregate(true, (current, device) => current && device.AudioEndpointVolume.Mute);
        var result = enabled ? MicrophoneMuteState.On : MicrophoneMuteState.Off;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(MicrophoneMuteState state)
    {
        var enabled = MicrophoneMuteState.On == state;
        foreach (var device in Devices)
            device.AudioEndpointVolume.Mute = enabled;
        return Task.CompletedTask;
    }
}
