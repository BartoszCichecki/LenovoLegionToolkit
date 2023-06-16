using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class PortsBacklightAutomationStep : AbstractFeatureAutomationStep<PortsBacklightState>
{
    [JsonConstructor]
    public PortsBacklightAutomationStep(PortsBacklightState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new PortsBacklightAutomationStep(State);
}
