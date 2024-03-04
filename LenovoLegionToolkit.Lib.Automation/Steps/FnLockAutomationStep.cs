using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class FnLockAutomationStep(FnLockState state)
    : AbstractFeatureAutomationStep<FnLockState>(state)
{
    public override IAutomationStep DeepCopy() => new FnLockAutomationStep(State);
}
