using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class FnLockAutomationStep : AbstractFeatureAutomationStep<FnLockState>
    {
        [JsonConstructor]
        public FnLockAutomationStep(FnLockState state) : base(state) { }

        public override IAutomationStep DeepCopy() => new FnLockAutomationStep(State);
    }
}
