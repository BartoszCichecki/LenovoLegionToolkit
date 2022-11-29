namespace LenovoLegionToolkit.Lib.Features;

public class TouchpadLockFeature : AbstractLenovoGamezoneWmiFeature<TouchpadLockState>
{
    public TouchpadLockFeature() : base("TPStatus", 0, "IsSupportDisableTP") { }
}