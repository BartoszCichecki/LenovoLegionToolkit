using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class TurnOnWiFiAutomationStepControl : AbstractAutomationStepControl
{
    public TurnOnWiFiAutomationStepControl(TurnOnWiFiAutomationStep automationStep) : base(automationStep)
    {
        Icon = SymbolRegular.Wifi124;
        Title = Resource.TurnOnWiFiAutomationStepControl_Title;
    }

    public override IAutomationStep CreateAutomationStep() => new TurnOnWiFiAutomationStep();

    protected override UIElement? GetCustomControl() => null;

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync() => Task.CompletedTask;
}
