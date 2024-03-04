using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class OverDriveAutomationStep(OverDriveState state)
    : AbstractFeatureAutomationStep<OverDriveState>(state)
{
    public override IAutomationStep DeepCopy() => new OverDriveAutomationStep(State);
}
