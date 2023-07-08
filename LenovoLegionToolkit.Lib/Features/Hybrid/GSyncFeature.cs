using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class GSyncFeature : AbstractWmiFeature<GSyncState>
{
    public GSyncFeature() : base(WMI.LenovoGameZoneData.GetGSyncStatusAsync, WMI.LenovoGameZoneData.SetGSyncStatusAsync, WMI.LenovoGameZoneData.IsSupportGSyncAsync) { }
}
