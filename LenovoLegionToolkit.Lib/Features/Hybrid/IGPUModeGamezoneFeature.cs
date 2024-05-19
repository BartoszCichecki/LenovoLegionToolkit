using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeGamezoneFeature()
    : AbstractWmiFeature<IGPUModeState>(WMI.LenovoGameZoneData.GetIGPUModeStatusAsync, WMI.LenovoGameZoneData.SetIGPUModeStatusAsync, WMI.LenovoGameZoneData.IsSupportIGPUModeAsync);
