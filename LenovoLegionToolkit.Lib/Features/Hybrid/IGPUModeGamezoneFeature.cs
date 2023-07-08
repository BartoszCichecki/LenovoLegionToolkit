using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeGamezoneFeature : AbstractWmiFeature<IGPUModeState>
{
    public IGPUModeGamezoneFeature() : base(WMI.LenovoGameZoneData.GetIGPUModeStatusAsync, WMI.LenovoGameZoneData.SetIGPUModeStatusAsync, WMI.LenovoGameZoneData.IsSupportIGPUModeAsync) { }
}
