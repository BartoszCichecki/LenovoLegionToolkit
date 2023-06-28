using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class InstantBootAutomationStep : AbstractFeatureAutomationStep<InstantBootState>
{
    [JsonConstructor]
    public InstantBootAutomationStep(InstantBootState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new InstantBootAutomationStep(State);
}
