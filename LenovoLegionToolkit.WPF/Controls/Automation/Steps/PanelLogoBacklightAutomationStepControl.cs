using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class PanelLogoBacklightAutomationStepControl : AbstractComboBoxAutomationStepCardControl<PanelLogoBacklightState>
{
    public PanelLogoBacklightAutomationStepControl(IAutomationStep<PanelLogoBacklightState> step) : base(step)
    {
        Icon = SymbolRegular.LightbulbCircle24;
        Title = Resource.PanelLogoBacklightAutomationStepControl_Title;
        Subtitle = Resource.PanelLogoBacklightAutomationStepControl_Message;
    }
}
