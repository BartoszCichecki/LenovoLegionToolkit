using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

internal class InstantBootAutomationStepControl : AbstractComboBoxAutomationStepCardControl<InstantBootState>
{
    public InstantBootAutomationStepControl(IAutomationStep<InstantBootState> step) : base(step)
    {
        Icon = SymbolRegular.PlugDisconnected24;
        Title = Resource.InstantBootAutomationStepControl_Title;
        Subtitle = Resource.InstantBootAutomationStepControl_Message;
    }
}
