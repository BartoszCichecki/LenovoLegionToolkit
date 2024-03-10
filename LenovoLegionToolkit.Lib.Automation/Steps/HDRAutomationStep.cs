using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class HDRAutomationStep(HDRState state)
    : AbstractFeatureAutomationStep<HDRState>(state)
{
    public override IAutomationStep DeepCopy() => new HDRAutomationStep(State);
}
