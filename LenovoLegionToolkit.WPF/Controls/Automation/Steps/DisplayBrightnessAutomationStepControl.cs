using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using NumberBox = Wpf.Ui.Controls.NumberBox;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class DisplayBrightnessAutomationStepControl : AbstractAutomationStepControl<DisplayBrightnessAutomationStep>
{
    private readonly NumberBox _brightness = new()
    {
        Width = 150,
        ClearButtonEnabled = false,
        MaxDecimalPlaces = 0,
        Minimum = 0,
        Maximum = 100,
        SmallChange = 5,
        LargeChange = 5
    };

    private readonly Grid _grid = new();

    public DisplayBrightnessAutomationStepControl(DisplayBrightnessAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.BrightnessHigh48;
        Title = Resource.DisplayBrightnessAutomationStepControl_Title;
        Subtitle = Resource.DisplayBrightnessAutomationStepControl_Message;
    }

    public override IAutomationStep CreateAutomationStep() => new DisplayBrightnessAutomationStep((int?)_brightness.Value ?? 0);

    protected override UIElement GetCustomControl()
    {
        _brightness.TextChanged += (_, _) =>
        {
            if ((int?)_brightness.Value != AutomationStep.Brightness)
                RaiseChanged();
        };
        _grid.Children.Add(_brightness);
        return _grid;
    }

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync()
    {
        _brightness.Value = AutomationStep.Brightness;
        return Task.CompletedTask;
    }
}
