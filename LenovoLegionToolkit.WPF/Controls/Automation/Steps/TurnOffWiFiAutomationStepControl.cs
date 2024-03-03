using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class TurnOffWiFiAutomationStepControl : AbstractAutomationStepControl
{
    public TurnOffWiFiAutomationStepControl(TurnOffWiFiAutomationStep automationStep) : base(automationStep)
    {
        Icon = SymbolRegular.WifiOff24;
        Title = Resource.TurnOffWiFiAutomationStepControl_Title;
    }

    public override IAutomationStep CreateAutomationStep() => new TurnOffWiFiAutomationStep();

    protected override UIElement? GetCustomControl() => null;

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync() => Task.CompletedTask;
}
