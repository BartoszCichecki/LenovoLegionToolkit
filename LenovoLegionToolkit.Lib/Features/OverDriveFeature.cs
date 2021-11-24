namespace LenovoLegionToolkit.Lib.Features
{
    public class OverDriveFeature : AbstractWmiFeature<OverDriveState>
    {
        public OverDriveFeature() : base("ODStatus", 0, "IsSupportOD") { }
    }
}