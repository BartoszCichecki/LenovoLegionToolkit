using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public static class HybridModeAutomationStepControlFactory
{
    public static async Task<AbstractAutomationStepControl<IAutomationStep<HybridModeState>>> GetControlAsync(IAutomationStep<HybridModeState> step)
    {
        var mi = await Compatibility.GetMachineInformationAsync();
        return mi.Properties.SupportsIGPUMode
            ? new ComboBoxHybridModeAutomationStepControl(step)
            : new ToggleHybridModeAutomationStepControl(step);
    }

    private class ComboBoxHybridModeAutomationStepControl : AbstractComboBoxAutomationStepCardControl<HybridModeState>
    {
        public ComboBoxHybridModeAutomationStepControl(IAutomationStep<HybridModeState> step) : base(step)
        {
            Icon = SymbolRegular.LeafOne24;
            Title = Resource.ComboBoxHybridModeAutomationStepControl_Title;
            Subtitle = Resource.ComboBoxHybridModeAutomationStepControl_Message;
        }
    }

    private class ToggleHybridModeAutomationStepControl : AbstractComboBoxAutomationStepCardControl<HybridModeState>
    {
        public ToggleHybridModeAutomationStepControl(IAutomationStep<HybridModeState> step) : base(step)
        {
            Icon = SymbolRegular.LeafOne24;
            Title = Resource.ToggleHybridModeAutomationStepControl_Title;
            Subtitle = Resource.ToggleHybridModeAutomationStepControl_Message;
        }
    }
}
