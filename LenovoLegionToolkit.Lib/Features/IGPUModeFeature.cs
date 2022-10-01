using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class IGPUModeFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
    {
        public IGPUModeFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode") { }

        public override async Task SetStateAsync(IGPUModeState state)
        {
            await base.SetStateAsync(state).ConfigureAwait(false);

            // Seem like maybe we should force Bindows to scan? Not sure tho
            await CMD.RunAsync("pnputil", "/scan-devices /async").ConfigureAwait(false);

            // HACK
            // await CMD.RunAsync("pnputil", "/scan-devices /async").ConfigureAwait(false);
            // await NotifyDGPUStatusAsync(state).ConfigureAwait(false);
        }

        public Task NotifyDGPUStatusAsync(IGPUModeState state) => WMI.CallAsync(Scope,
            Query,
            "NotifyDGPUStatus",
            new() { { "Status", ToInternal(state).ToString() } });
    }
}