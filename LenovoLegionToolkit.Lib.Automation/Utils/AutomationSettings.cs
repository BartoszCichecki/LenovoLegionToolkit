using System.Collections.Generic;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Automation.Utils;

public class AutomationSettings : AbstractSettings<AutomationSettings.AutomationSettingsStore>
{
    public class AutomationSettingsStore
    {
        public bool IsEnabled { get; set; }

        public List<AutomationPipeline> Pipelines { get; set; } = new();
    }

    protected override string FileName => "automation.json";

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
                Name = "Deactivate GPU",
                Steps = { new DeactivateGPUAutomationStep(DeactivateGPUAutomationStepState.KillApps) },
            },
        },
    };
}