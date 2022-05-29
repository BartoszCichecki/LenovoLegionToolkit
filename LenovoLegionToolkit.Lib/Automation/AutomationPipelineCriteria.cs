using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Automation
{
    internal enum AutomationPipelineCriteria
    {
        ACAdapterConnected,
        ACAdapterDisconnected,
        DisplayConfigurationChanged,
    }

    internal static class AutomationPipelineCriteriaExtensions
    {
        public static bool IsSatisfied(this AutomationPipelineCriteria trigger)
        {
            return trigger switch
            {
                AutomationPipelineCriteria.ACAdapterConnected => Power.IsPowerAdapterConnected(),
                AutomationPipelineCriteria.ACAdapterDisconnected => !Power.IsPowerAdapterConnected(),
                AutomationPipelineCriteria.DisplayConfigurationChanged => true,
                _ => false,
            };
        }
    }
}
