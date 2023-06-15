namespace LenovoLegionToolkit.Lib.Features;

public class PortsBacklightFeature : AbstractLenovoLightingFeature<PortsBacklightState>
{
    public PortsBacklightFeature() : base(5) { }

    protected override PortsBacklightState FromInternal(int value) => (PortsBacklightState)value;

    protected override int ToInternal(PortsBacklightState state) => (int)state;
}
