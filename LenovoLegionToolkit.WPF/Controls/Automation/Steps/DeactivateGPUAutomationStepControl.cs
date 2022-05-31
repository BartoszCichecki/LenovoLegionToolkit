using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Steps;
using WPFUI.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class DeactivateGPUAutomationStepControl : AbstractAutomationStepControl
    {
        public override IAutomationStep AutomationStep { get; }

        public DeactivateGPUAutomationStepControl(DeactivateGPUAutomationStep step)
        {
            AutomationStep = step;

            Icon = SymbolRegular.DeveloperBoard24;
            Title = "Deactivate GPU";
            Subtitle = "Automatically deactivate GPU.";
        }

        protected override UIElement? CustomControl => null;

        protected override void OnFinishedLoading() { }

        protected override Task RefreshAsync() => Task.CompletedTask;
    }
}
