using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class WinKeyAutomationStep(WinKeyState state)
    : AbstractFeatureAutomationStep<WinKeyState>(state)
{
    public override IAutomationStep DeepCopy() => new WinKeyAutomationStep(State);
}
