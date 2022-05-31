using System.ComponentModel.DataAnnotations;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline
{
    public enum AutomationPipelineTrigger
    {
        [Display(Name = "AC adapter is connected")]
        ACAdapterConnected,
        [Display(Name = "AC adapter is disconnected")]
        ACAdapterDisconnected,
    }

    internal static class AutomationPipelineTriggerExtensions
    {
        public static bool IsSatisfied(this AutomationPipelineTrigger trigger)
        {
            return trigger switch
            {
                AutomationPipelineTrigger.ACAdapterConnected => Power.IsPowerAdapterConnected(),
                AutomationPipelineTrigger.ACAdapterDisconnected => !Power.IsPowerAdapterConnected(),
                _ => false,
            };
        }
    }
}
