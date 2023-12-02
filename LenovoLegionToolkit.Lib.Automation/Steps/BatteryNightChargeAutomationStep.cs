using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class BatteryNightChargeAutomationStep : AbstractFeatureAutomationStep<BatteryNightChargeState>
{
    [JsonConstructor]
    public BatteryNightChargeAutomationStep(BatteryNightChargeState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new BatteryNightChargeAutomationStep(State);
}
