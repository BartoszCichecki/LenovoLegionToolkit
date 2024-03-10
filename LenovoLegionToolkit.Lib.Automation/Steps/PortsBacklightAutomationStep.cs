using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class PortsBacklightAutomationStep(PortsBacklightState state)
    : AbstractFeatureAutomationStep<PortsBacklightState>(state)
{
    public override IAutomationStep DeepCopy() => new PortsBacklightAutomationStep(State);
}
