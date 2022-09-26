using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class WhiteKeyboardBacklightAutomationStep : AbstractFeatureAutomationStep<WhiteKeyboardBacklightState>
    {
        [JsonConstructor]
        public WhiteKeyboardBacklightAutomationStep(WhiteKeyboardBacklightState state) : base(state) { }

        public override async Task RunAsync()
        {
            await Feature.SetStateAsync(State).ConfigureAwait(false);
            MessagingCenter.Publish(State);
        }

        public override IAutomationStep DeepCopy() => new WhiteKeyboardBacklightAutomationStep(State);
    }
}
