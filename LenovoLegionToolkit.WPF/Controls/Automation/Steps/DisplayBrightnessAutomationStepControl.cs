using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;
using NumberBox = Wpf.Ui.Controls.NumberBox;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class DisplayBrightnessAutomationStepControl : AbstractAutomationStepControl<DisplayBrightnessAutomationStep>
    {
        private readonly NumberBox _brightness = new()
        {
            Width = 150,
            IntegersOnly = true,
            ClearButtonEnabled = false,
            Min = 0,
            Max = 100,
            Step = 5,
        };

        private readonly Grid _grid = new();

        public DisplayBrightnessAutomationStepControl(DisplayBrightnessAutomationStep step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh48;
            Title = "Display brightness";
            Subtitle = "Change display brightness of the built-in display.\nPower modes change brightness on some devices. Make sure to put this step last, if something doesn't work just right.\n\nWARNING: This action will not run correctly, if internal display is off.";
        }

        public override IAutomationStep CreateAutomationStep() => new DisplayBrightnessAutomationStep((int)_brightness.Value);

        protected override UIElement? GetCustomControl()
        {
            _brightness.TextChanged += (s, e) => RaiseChanged();
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
}
