using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class PanelLogoBacklightAutomationStep : AbstractFeatureAutomationStep<PanelLogoBacklightState>
{
    [JsonConstructor]
    public PanelLogoBacklightAutomationStep(PanelLogoBacklightState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new PanelLogoBacklightAutomationStep(State);
}
