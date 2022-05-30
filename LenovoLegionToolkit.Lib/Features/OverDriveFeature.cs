namespace LenovoLegionToolkit.Lib.Features
{
    public class OverDriveFeature : AbstractWmiFeature<OverDriveState>
    {
        internal OverDriveFeature() : base("ODStatus", 0, "IsSupportOD") { }
    }
}