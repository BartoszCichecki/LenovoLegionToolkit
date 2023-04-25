using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class HDRAutomationStep : AbstractFeatureAutomationStep<HDRState>
{
    [JsonConstructor]
    public HDRAutomationStep(HDRState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new HDRAutomationStep(State);
}
