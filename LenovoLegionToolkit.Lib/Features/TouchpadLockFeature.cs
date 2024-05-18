using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features;

public class TouchpadLockFeature()
    : AbstractWmiFeature<TouchpadLockState>(WMI.LenovoGameZoneData.GetTPStatusStatusAsync, WMI.LenovoGameZoneData.SetTPStatusAsync, WMI.LenovoGameZoneData.IsSupportDisableTPAsync);
