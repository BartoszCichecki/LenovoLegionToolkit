using System.Collections.Generic;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Automation.Utils;

public class AutomationSettings() : AbstractSettings<AutomationSettings.AutomationSettingsStore>("automation.json")
{
    public class AutomationSettingsStore
    {
        public bool IsEnabled { get; set; }

        public List<AutomationPipeline> Pipelines { get; set; } = [];
    }

    protected override AutomationSettingsStore Default => new()
    {
        Pipelines =
        {
            new AutomationPipeline
            {
                Trigger = new ACAdapterConnectedAutomationPipelineTrigger(),
                Steps = { new PowerModeAutomationStep(PowerModeState.Balance) },
            },
            new AutomationPipeline
            {
                Trigger = new ACAdapterDisconnectedAutomationPipelineTrigger(),
                Steps = { new PowerModeAutomationStep(PowerModeState.Quiet) },
            },
            new AutomationPipeline
            {
                Name = Resource.DeactivateGpuQuickAction_Title,
                Steps = { new DeactivateGPUAutomationStep(DeactivateGPUAutomationStepState.KillApps) },
            },
        },
    };
}
