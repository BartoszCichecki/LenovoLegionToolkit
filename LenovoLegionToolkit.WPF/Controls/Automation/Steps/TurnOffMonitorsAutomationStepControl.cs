using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class TurnOffMonitorsAutomationStepControl : AbstractAutomationStepControl
{
    public TurnOffMonitorsAutomationStepControl(IAutomationStep automationStep) : base(automationStep)
    {
        Icon = SymbolRegular.Desktop24.GetIcon();
        Title = Resource.TurnOffMonitorsAutomationStepControl_Title;
        Subtitle = Resource.TurnOffMonitorsAutomationStepControl_Message;
    }

    public override IAutomationStep CreateAutomationStep() => new TurnOffMonitorsAutomationStep();

    protected override UIElement? GetCustomControl() => null;

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync() => Task.CompletedTask;
}
