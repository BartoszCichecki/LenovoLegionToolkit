using System.Collections.Generic;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.Lib.Automation.Utils
{
    public class AutomationSettings : AbstractSettings<AutomationSettings.AutomationSettingsStore>
    {
        public class AutomationSettingsStore
        {
            public bool IsEnabled { get; set; } = false;

            public List<AutomationPipeline> Pipelines { get; set; } = new()
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
                    Steps = { new DeactivateGPUAutomationStep() },
                },
            };
        }

        protected override string FileName => "automation.json";
    }
}
