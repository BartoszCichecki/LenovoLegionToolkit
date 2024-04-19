using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class SpeakerSystemDefaultDeviceStep(SpeakerSystemDefaultDeviceState state)
    : AbstractFeatureAutomationStep<SpeakerSystemDefaultDeviceState>(state)
{
    public override IAutomationStep DeepCopy() => new SpeakerSystemDefaultDeviceStep(State);
}
