using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features;

public class TouchpadLockFeature : AbstractWmiFeature<TouchpadLockState>
{
    public TouchpadLockFeature() : base(WMI.LenovoGameZoneData.GetTPStatusStatusAsync, WMI.LenovoGameZoneData.SetTPStatusAsync, WMI.LenovoGameZoneData.IsSupportDisableTPAsync) { }
}
