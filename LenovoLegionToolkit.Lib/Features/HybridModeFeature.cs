namespace LenovoLegionToolkit.Lib.Features
{
    public class HybridModeFeature : AbstractWmiFeature<HybridModeState>
    {
        public HybridModeFeature() : base("GSyncStatus", 0, "IsSupportGSync") { }
    }
}