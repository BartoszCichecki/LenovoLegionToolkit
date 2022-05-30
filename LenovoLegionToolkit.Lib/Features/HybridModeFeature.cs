namespace LenovoLegionToolkit.Lib.Features
{
    public class HybridModeFeature : AbstractWmiFeature<HybridModeState>
    {
        internal HybridModeFeature() : base("GSyncStatus", 0, "IsSupportGSync") { }
    }
}