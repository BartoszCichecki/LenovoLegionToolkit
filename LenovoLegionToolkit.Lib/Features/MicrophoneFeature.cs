using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using NAudio.CoreAudioApi;

namespace LenovoLegionToolkit.Lib.Features;

public class MicrophoneFeature : IFeature<MicrophoneState>
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private IEnumerable<AudioEndpointVolume> AudioEndpointVolumes => _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Select(d => d.AudioEndpointVolume);

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

    public Task<MicrophoneState[]> GetAllStatesAsync() => Task.FromResult(Enum.GetValues<MicrophoneState>());

    public Task<MicrophoneState> GetStateAsync()
    {
        var mute = AudioEndpointVolumes.Aggregate(true, (current, v) => current && v.Mute);
        var result = mute ? MicrophoneState.Off : MicrophoneState.On;
        return Task.FromResult(result);
    }

    public Task SetStateAsync(MicrophoneState state)
    {
        var mute = MicrophoneState.Off == state;
        AudioEndpointVolumes.ForEach(v => v.Mute = mute);
        return Task.CompletedTask;
    }
}
