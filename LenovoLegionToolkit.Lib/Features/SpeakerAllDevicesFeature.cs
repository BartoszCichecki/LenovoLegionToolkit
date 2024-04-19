using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class SpeakerAllDevicesFeature :IFeature<SpeakerAllDevicesState>
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

    public Task<SpeakerAllDevicesState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<SpeakerAllDevicesState>());

    public Task<SpeakerAllDevicesState> GetStateAsync()
    {
        var mute = AudioEndpointVolumes.Aggregate(true, (current, v) => current && v.Mute);
        var result = mute ? SpeakerAllDevicesState.Mute : SpeakerAllDevicesState.Unmute;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(SpeakerAllDevicesState state)
    {
        var mute = SpeakerAllDevicesState.Mute == state;
        AudioEndpointVolumes.ForEach(v => v.Mute = mute);
        return Task.CompletedTask;
    }
}
