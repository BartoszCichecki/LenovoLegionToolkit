using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class MicrophoneAutomationStep : AbstractFeatureAutomationStep<MicrophoneState>
{
    [JsonConstructor]
    public MicrophoneAutomationStep(MicrophoneState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new MicrophoneAutomationStep(State);
}
