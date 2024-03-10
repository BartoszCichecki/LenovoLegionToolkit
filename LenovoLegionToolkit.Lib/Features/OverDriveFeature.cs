using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features;

public class OverDriveFeature() : AbstractWmiFeature<OverDriveState>(WMI.LenovoGameZoneData.GetODStatusAsync,
    WMI.LenovoGameZoneData.SetODStatusAsync, WMI.LenovoGameZoneData.IsSupportODAsync);
