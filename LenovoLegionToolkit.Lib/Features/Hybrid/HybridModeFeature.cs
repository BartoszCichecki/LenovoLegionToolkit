using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class HybridModeFeature : IFeature<HybridModeState>
{
    private readonly GSyncFeature _gSyncFeature;
    private readonly IGPUModeFeature _igpuModeFeature;

    public HybridModeFeature(GSyncFeature gSyncFeature, IGPUModeFeature igpuModeFeature)
    {
        _gSyncFeature = gSyncFeature ?? throw new ArgumentNullException(nameof(gSyncFeature));
        _igpuModeFeature = igpuModeFeature ?? throw new ArgumentNullException(nameof(igpuModeFeature));
    }

    public Task<bool> IsSupportedAsync() => _gSyncFeature.IsSupportedAsync();

    public async Task<HybridModeState[]> GetAllStatesAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        return mi.Properties.SupportsIGPUMode
            ? new[] { HybridModeState.On, HybridModeState.OnIGPUOnly, HybridModeState.OnAuto, HybridModeState.Off }
            : new[] { HybridModeState.On, HybridModeState.Off };
    }

    public async Task<HybridModeState> GetStateAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Getting state...");

        var gSync = await _gSyncFeature.GetStateAsync().ConfigureAwait(false);

        var igpuMode = IGPUModeState.Default;
        if (await _igpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
            igpuMode = await _igpuModeFeature.GetStateAsync().ConfigureAwait(false);

        var state = Pack(gSync, igpuMode);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State is {state} [gSync={gSync}, igpuMode={igpuMode}]");

        return state;
    }

    public async Task SetStateAsync(HybridModeState state)
    {
        var (gSync, igpuMode) = Unpack(state);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Setting state to {state}... [gSync={gSync}, igpuMode={igpuMode}]");

        if (await _gSyncFeature.GetStateAsync().ConfigureAwait(false) != gSync)
            await _gSyncFeature.SetStateAsync(gSync).ConfigureAwait(false);

        if (await _igpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
        {
            if (await _igpuModeFeature.GetStateAsync().ConfigureAwait(false) != igpuMode)
                await _igpuModeFeature.SetStateAsync(igpuMode).ConfigureAwait(false);
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"State set to {state} [gSync={gSync}, igpuMode={igpuMode}]");
    }

    private static (GSyncState, IGPUModeState) Unpack(HybridModeState state) => state switch
    {
        HybridModeState.On => (GSyncState.On, IGPUModeState.Default),
        HybridModeState.OnIGPUOnly => (GSyncState.On, IGPUModeState.IGPUOnly),
        HybridModeState.OnAuto => (GSyncState.On, IGPUModeState.Auto),
        HybridModeState.Off => (GSyncState.Off, IGPUModeState.Default),
        _ => throw new InvalidOperationException("Invalid state"),
    };

    private static HybridModeState Pack(GSyncState state1, IGPUModeState state2) => (state1, state2) switch
    {
        (GSyncState.On, IGPUModeState.Default) => HybridModeState.On,
        (GSyncState.On, IGPUModeState.IGPUOnly) => HybridModeState.OnIGPUOnly,
        (GSyncState.On, IGPUModeState.Auto) => HybridModeState.OnAuto,
        (GSyncState.Off, _) => HybridModeState.Off,
        _ => throw new InvalidOperationException("Invalid state"),
    };
}
