using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class TouchpadLockAutomationStep : AbstractFeatureAutomationStep<TouchpadLockState>
{
    [JsonConstructor]
    public TouchpadLockAutomationStep(TouchpadLockState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new TouchpadLockAutomationStep(State);
}
