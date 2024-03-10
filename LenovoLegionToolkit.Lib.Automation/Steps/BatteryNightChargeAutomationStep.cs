using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class BatteryNightChargeAutomationStep(BatteryNightChargeState state)
    : AbstractFeatureAutomationStep<BatteryNightChargeState>(state)
{
    public override IAutomationStep DeepCopy() => new BatteryNightChargeAutomationStep(State);
}
