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
            PlaceholderText = "Brightness value",
            Width = 150,
            IntegersOnly = true,
            Step = 5,
            Max = 100,
            Min = 0,
            Value = 50,
        };

        private readonly StackPanel _stackPanel = new();

        public DisplayBrightnessAutomationStepControl(DisplayBrightnessAutomationStep step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh48;
            Title = "Display brightness";
            Subtitle = "Change display brightness of the built-in display.\n\nWARNING: This action will not run correctly,\nif internal display is off.";
        }

        public override IAutomationStep CreateAutomationStep() => new DisplayBrightnessAutomationStep((int)_brightness.Value);

        protected override UIElement? GetCustomControl()
        {
            _brightness.TextChanged += (s, e) =>
            {
                if (_brightness.Value != AutomationStep.Brightness)
                    RaiseChanged();
            };

            _stackPanel.Children.Add(_brightness);

            return _stackPanel;
        }

        protected override void OnFinishedLoading() { }

        protected override Task RefreshAsync()
        {
            _brightness.Value = AutomationStep.Brightness;
            return Task.CompletedTask;
        }
    }
}
