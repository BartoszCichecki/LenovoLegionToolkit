using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class PanelLogoBacklightAutomationStep(PanelLogoBacklightState state)
    : AbstractFeatureAutomationStep<PanelLogoBacklightState>(state)
{
    public override IAutomationStep DeepCopy() => new PanelLogoBacklightAutomationStep(State);
}
