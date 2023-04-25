using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class ResolutionAutomationStep : AbstractFeatureAutomationStep<Resolution>
{
    [JsonConstructor]
    public ResolutionAutomationStep(Resolution state) : base(state) { }

    public override IAutomationStep DeepCopy() => new ResolutionAutomationStep(State);
}
