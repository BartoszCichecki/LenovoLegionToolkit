using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class PowerModeAutomationStep : AbstractFeatureAutomationStep<PowerModeState>
{
    [JsonConstructor]
    public PowerModeAutomationStep(PowerModeState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new PowerModeAutomationStep(State);
}
