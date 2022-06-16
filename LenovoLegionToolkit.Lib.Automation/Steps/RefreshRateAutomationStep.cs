using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class RefreshRateAutomationStep : AbstractFeatureAutomationStep<RefreshRate>
    {
        [JsonConstructor]
        public RefreshRateAutomationStep(RefreshRate state) : base(state) { }

        public override IAutomationStep DeepCopy() => new RefreshRateAutomationStep(State);
    }
}
