namespace LenovoLegionToolkit.Lib.Features
{
    public class OverDriveFeature : AbstractLenovoGamezoneWmiFeature<OverDriveState>
    {
        public OverDriveFeature() : base("ODStatus", 0, "IsSupportOD") { }
    }
}