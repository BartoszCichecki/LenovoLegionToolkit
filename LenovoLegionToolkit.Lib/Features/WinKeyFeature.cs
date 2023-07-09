using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Lib.Features;

public class WinKeyFeature : AbstractWmiFeature<WinKeyState>
{
    public WinKeyFeature() : base(WMI.LenovoGameZoneData.GetWinKeyStatusAsync, WMI.LenovoGameZoneData.SetWinKeyStatusAsync, WMI.LenovoGameZoneData.IsSupportDisableWinKeyAsync) { }
}
