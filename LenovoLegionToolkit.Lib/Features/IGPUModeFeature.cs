namespace LenovoLegionToolkit.Lib.Features
{
    public class IGPUModeFeature : AbstractLenovoGamezoneWmiFeature<IGPUModeState>
    {
        public IGPUModeFeature() : base("IGPUModeStatus", 0, "IsSupportIGPUMode", inParameterName: "mode", notifyUpdateMethodName: "NotifyDGPUStatus", notifyUpdateInParameterNameName: "Status") { }
    }
}