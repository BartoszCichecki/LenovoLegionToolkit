using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class SpeakerFeature : IFeature<SpeakerState>
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private IEnumerable<AudioEndpointVolume> AudioEndpointVolumes => _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Select(d => d.AudioEndpointVolume);

    public Task<bool> IsSupportedAsync()
    {
        try
        {
            var isSupported = AudioEndpointVolumes.Any();
            return Task.FromResult(isSupported);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<SpeakerState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<SpeakerState>());

    public Task<SpeakerState> GetStateAsync()
    {
        var mute = AudioEndpointVolumes.Aggregate(true, (current, v) => current && v.Mute);
        var result = mute ? SpeakerState.Mute : SpeakerState.Unmute;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(SpeakerState state)
    {
        var mute = SpeakerState.Mute == state;
        AudioEndpointVolumes.ForEach(v => v.Mute = mute);
        return Task.CompletedTask;
    }
}
