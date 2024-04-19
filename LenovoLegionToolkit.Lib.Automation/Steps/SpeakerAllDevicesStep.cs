using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class SpeakerAllDevicesStep(SpeakerAllDevicesState state)
    : AbstractFeatureAutomationStep<SpeakerAllDevicesState>(state)
{
    public override IAutomationStep DeepCopy() => new SpeakerAllDevicesStep(State);
}
