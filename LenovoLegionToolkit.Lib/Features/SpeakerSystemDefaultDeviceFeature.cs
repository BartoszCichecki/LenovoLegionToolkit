using System;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class SpeakerSystemDefaultDeviceFeature : IFeature<SpeakerSystemDefaultDeviceState>
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private AudioEndpointVolume DefaultAudioEndpointVolume => _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioEndpointVolume;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task<SpeakerSystemDefaultDeviceState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<SpeakerSystemDefaultDeviceState>());

    public Task<SpeakerSystemDefaultDeviceState> GetStateAsync()
    {
        var mute = DefaultAudioEndpointVolume.Mute;
        var result = mute ? SpeakerSystemDefaultDeviceState.Mute : SpeakerSystemDefaultDeviceState.Unmute;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(SpeakerSystemDefaultDeviceState state)
    {
        var mute = SpeakerSystemDefaultDeviceState.Mute == state;
        DefaultAudioEndpointVolume.Mute = mute;
        return Task.CompletedTask;
    }
}
