namespace LenovoLegionToolkit.Lib.Features
{
    public enum HybridModeState
    {
        On,
        Off
    }

    public class HybridModeFeature : AbstractWmiFeature<HybridModeState>
    {
        public HybridModeFeature() : base("GSyncStatus", 0)
        {
        }
    }
}