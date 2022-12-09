using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class ScreenDPIAutomationStepControl : AbstractComboBoxAutomationStepCardControl<ScreenDPI>
    {
        public ScreenDPIAutomationStepControl(IAutomationStep<ScreenDPI> dpi) : base(dpi)
        {
            Icon = SymbolRegular.Laptop24;
            Title = Resource.ScreenDPIAutomationStepControl_Title;
            Subtitle = Resource.ScreenDPIAutomationStepControl_Message;
        }
    }
}
