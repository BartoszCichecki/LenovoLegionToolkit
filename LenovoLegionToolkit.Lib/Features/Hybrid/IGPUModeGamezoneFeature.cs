namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeGamezoneFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
{
    public IGPUModeGamezoneFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode") { }
}
