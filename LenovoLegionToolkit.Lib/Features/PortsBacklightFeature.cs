namespace LenovoLegionToolkit.Lib.Features;

public class PortsBacklightFeature() : AbstractLenovoLightingFeature<PortsBacklightState>(5, 1, 0)
{
    protected override PortsBacklightState FromInternal(int stateType, int _) => (PortsBacklightState)stateType;

    protected override (int stateType, int level) ToInternal(PortsBacklightState state) => ((int)state, 0);
}
