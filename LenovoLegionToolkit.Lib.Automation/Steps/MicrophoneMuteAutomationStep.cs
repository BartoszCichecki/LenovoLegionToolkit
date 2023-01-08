using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class MicrophoneMuteAutomationStep : AbstractFeatureAutomationStep<MicrophoneMuteState>
{
    [JsonConstructor]
    public MicrophoneMuteAutomationStep(MicrophoneMuteState state) : base(state) { }

    public override IAutomationStep DeepCopy() => new MicrophoneMuteAutomationStep(State);
}
