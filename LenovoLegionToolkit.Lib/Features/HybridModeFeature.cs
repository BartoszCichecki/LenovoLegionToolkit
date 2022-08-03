namespace LenovoLegionToolkit.Lib.Features
{
    public class HybridModeFeature : AbstractLenovoGamezoneWmiFeature<HybridModeState>
    {
        public HybridModeFeature() : base("GSyncStatus", 0, "IsSupportGSync") { }
    }
}