namespace LenovoLegionToolkit.Lib.Features
{

    public class TouchpadLockFeature : AbstractWmiFeature<TouchpadLockState>
    {
        internal TouchpadLockFeature() : base("TPStatus", 0, "IsSupportDisableTP") { }
    }
}