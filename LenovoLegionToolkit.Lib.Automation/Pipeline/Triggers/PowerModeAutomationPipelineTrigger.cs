using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class PowerModeAutomationPipelineTrigger : IAutomationPipelineTrigger, IPowerModeAutomationPipelineTrigger
    {
        public string DisplayName => "When power mode is changed";

        public PowerModeState PowerModeState { get; }

        [JsonConstructor]
        public PowerModeAutomationPipelineTrigger(PowerModeState powerModeState)
        {
            PowerModeState = powerModeState;
        }

        public async Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            if (automationEvent is StartupAutomationEvent)
                return false;

            if (automationEvent is not PowerModeAutomationEvent pmae)
                return false;

            return pmae.PowerModeState == PowerModeState;
        }

        public IAutomationPipelineTrigger DeepCopy() => new PowerModeAutomationPipelineTrigger(PowerModeState);

        public IAutomationPipelineTrigger DeepCopy(PowerModeState powerModeState) => new PowerModeAutomationPipelineTrigger(powerModeState);
    }
}
