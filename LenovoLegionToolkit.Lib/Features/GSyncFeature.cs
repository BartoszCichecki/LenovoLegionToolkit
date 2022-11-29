namespace LenovoLegionToolkit.Lib.Features;

public class GSyncFeature : AbstractLenovoGamezoneWmiFeature<GSyncState>
{
    public GSyncFeature() : base("GSyncStatus", 0, "IsSupportGSync") { }
}