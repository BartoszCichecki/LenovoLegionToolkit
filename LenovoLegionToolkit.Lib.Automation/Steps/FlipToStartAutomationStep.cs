using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class FlipToStartAutomationStep : AbstractFeatureAutomationStep<FlipToStartState>
{
    [JsonConstructor]
    public FlipToStartAutomationStep(FlipToStartState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new FlipToStartAutomationStep(State);
}
