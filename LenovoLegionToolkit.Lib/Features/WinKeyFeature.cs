namespace LenovoLegionToolkit.Lib.Features
{
    public class WinKeyFeature : AbstractWmiFeature<WinKeyState>
    {
        public WinKeyFeature() : base("WinKeyStatus", 0, "IsSupportDisableWinKey") { }
    }
}
