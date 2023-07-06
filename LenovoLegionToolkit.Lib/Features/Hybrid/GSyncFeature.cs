namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class GSyncFeature : AbstractLenovoGamezoneWmiFeature<GSyncState>
{
    public GSyncFeature() : base("GSyncStatus", 0, "IsSupportGSync") { }
}
