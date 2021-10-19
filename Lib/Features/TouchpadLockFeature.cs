namespace LenovoLegionToolkit.Lib.Features
{
    public enum TouchpadLockState
    {
        Off,
        On
    }

    public class TouchpadLockFeature : AbstractWmiFeature<TouchpadLockState>
    {
        public TouchpadLockFeature() : base("TPStatus", 0)
        {
        }
    }
}