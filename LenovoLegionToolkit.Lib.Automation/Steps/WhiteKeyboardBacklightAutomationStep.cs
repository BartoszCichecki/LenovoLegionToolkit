using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class WhiteKeyboardBacklightAutomationStep : AbstractFeatureAutomationStep<WhiteKeyboardBacklightState>
{
    [JsonConstructor]
    public WhiteKeyboardBacklightAutomationStep(WhiteKeyboardBacklightState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new WhiteKeyboardBacklightAutomationStep(State);
}