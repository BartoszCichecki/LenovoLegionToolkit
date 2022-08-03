namespace LenovoLegionToolkit.Lib.Features
{
    public class WinKeyFeature : AbstractLenovoGamezoneWmiFeature<WinKeyState>
    {
        public WinKeyFeature() : base("WinKeyStatus", 0, "IsSupportDisableWinKey") { }
    }
}
