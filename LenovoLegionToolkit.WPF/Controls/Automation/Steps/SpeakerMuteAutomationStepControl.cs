using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class SpeakerMuteAutomationStepControl : AbstractAutomationStepControl
{
    public SpeakerMuteAutomationStepControl(SpeakerMuteAutomationStep automationStep) : base(automationStep)
    {
        Icon = SymbolRegular.SpeakerMute24;
        Title = Resource.SpeakerMuteAutomationStepControl_Title;
    }

    public override IAutomationStep CreateAutomationStep() => new SpeakerMuteAutomationStep();

    protected override UIElement? GetCustomControl() => null;

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync() => Task.CompletedTask;
}
