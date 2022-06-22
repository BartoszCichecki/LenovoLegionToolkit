using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class DeactivateGPUAutomationStepControl : AbstractComboBoxAutomationStepCardControl<DeactivateGPUAutomationStepState>
    {
        public DeactivateGPUAutomationStepControl(DeactivateGPUAutomationStep step) : base(step)
        {
            Icon = SymbolRegular.DeveloperBoard24;
            Title = "Deactivate GPU";
            Subtitle = "Disable discrete GPU if it is active unnecessarily.\n\nWARNING: This action will not run correctly,\nif internal display is off or Hybrid mode is not active.";
        }
    }
}
