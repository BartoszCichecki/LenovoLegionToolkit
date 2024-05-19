using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features.OverDrive;

public class OverDriveGameZoneFeature()
    : AbstractWmiFeature<OverDriveState>(WMI.LenovoGameZoneData.GetODStatusAsync, WMI.LenovoGameZoneData.SetODStatusAsync, WMI.LenovoGameZoneData.IsSupportODAsync);
