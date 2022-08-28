using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features
{
    public class HybridModeFeature : IFeature<HybridModeState>
    {
        private readonly GSyncFeature _gsyncFeature;
        private readonly IGPUModeFeature _igpuModeFeature;

        public HybridModeFeature(GSyncFeature gsyncFeature, IGPUModeFeature igpuModeFeature)
        {
            _gsyncFeature = gsyncFeature ?? throw new ArgumentNullException(nameof(gsyncFeature));
            _igpuModeFeature = igpuModeFeature ?? throw new ArgumentNullException(nameof(igpuModeFeature));
        }

        public async Task<HybridModeState[]> GetAllStatesAsync()
        {
            var mi = await Compatibility.GetMachineInformation().ConfigureAwait(false);
            if (mi.Properties.SupportsExtendedHybridMode)
                return new[] { HybridModeState.On, HybridModeState.OnIGPUOnly, HybridModeState.OnAuto, HybridModeState.Off };
            else
                return new[] { HybridModeState.On, HybridModeState.Off };
        }

        public async Task<HybridModeState> GetStateAsync()
        {
            var gsync = await _gsyncFeature.GetStateAsync().ConfigureAwait(false);

            var igpuMode = IGPUModeState.Default;
            if (await _igpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
                igpuMode = await _igpuModeFeature.GetStateAsync().ConfigureAwait(false);

            return Pack(gsync, igpuMode);
        }

        public async Task SetStateAsync(HybridModeState state)
        {
            var (gsync, igpuMode) = Unpack(state);

            if (await _igpuModeFeature.IsSupportedAsync().ConfigureAwait(false))
            {
                if (igpuMode != await _igpuModeFeature.GetStateAsync().ConfigureAwait(false))
                    await _igpuModeFeature.SetStateAsync(igpuMode).ConfigureAwait(false);
            }

            if (gsync != await _gsyncFeature.GetStateAsync().ConfigureAwait(false))
                await _gsyncFeature.SetStateAsync(gsync).ConfigureAwait(false);
        }

        private (GSyncState, IGPUModeState) Unpack(HybridModeState state) => state switch
        {
            HybridModeState.On => (GSyncState.On, IGPUModeState.Default),
            HybridModeState.OnIGPUOnly => (GSyncState.On, IGPUModeState.IGPUOnly),
            HybridModeState.OnAuto => (GSyncState.On, IGPUModeState.Auto),
            HybridModeState.Off => (GSyncState.Off, IGPUModeState.Default),
            _ => throw new InvalidOperationException("Invalid state"),
        };

        private HybridModeState Pack(GSyncState state1, IGPUModeState state2) => (state1, state2) switch
        {
            (GSyncState.On, IGPUModeState.Default) => HybridModeState.On,
            (GSyncState.On, IGPUModeState.IGPUOnly) => HybridModeState.OnIGPUOnly,
            (GSyncState.On, IGPUModeState.Auto) => HybridModeState.OnAuto,
            (GSyncState.Off, IGPUModeState.Default) => HybridModeState.Off,
            _ => throw new InvalidOperationException("Invalid state"),
        };
    }
}
