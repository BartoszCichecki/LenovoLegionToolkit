using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features;

public class OverDriveFeature : AbstractWmiFeature<OverDriveState>
{
    public OverDriveFeature() : base(WMI.LenovoGameZoneData.GetODStatusAsync, WMI.LenovoGameZoneData.SetODStatusAsync, WMI.LenovoGameZoneData.IsSupportODAsync) { }
}
