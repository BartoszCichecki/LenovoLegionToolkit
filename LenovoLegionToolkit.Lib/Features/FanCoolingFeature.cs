namespace LenovoLegionToolkit.Lib.Features
{
    public class FanCoolingFeature : AbstractWmiFeature<FanCoolingState>
    {
        public FanCoolingFeature() : base("FanCooling", 0, getMethodNameSuffix: "FanCoolingState") { }
    }
}