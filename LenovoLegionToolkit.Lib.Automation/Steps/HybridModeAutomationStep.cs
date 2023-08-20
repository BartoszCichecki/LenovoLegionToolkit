using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class HybridModeAutomationStep : AbstractFeatureAutomationStep<HybridModeState>
{
    [JsonConstructor]
    public HybridModeAutomationStep(HybridModeState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new HybridModeAutomationStep(State);
}
