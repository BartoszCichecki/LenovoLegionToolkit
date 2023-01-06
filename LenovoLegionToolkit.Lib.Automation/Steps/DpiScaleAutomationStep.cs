using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class DpiScaleAutomationStep : AbstractFeatureAutomationStep<DpiScale>
{
    [JsonConstructor]
    public DpiScaleAutomationStep(DpiScale state) : base(state) { }

    public override IAutomationStep DeepCopy() => new DpiScaleAutomationStep(State);
}