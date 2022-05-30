namespace LenovoLegionToolkit.Lib.Features
{

    public class TouchpadLockFeature : AbstractWmiFeature<TouchpadLockState>
    {
        public TouchpadLockFeature() : base("TPStatus", 0, "IsSupportDisableTP") { }
    }
}