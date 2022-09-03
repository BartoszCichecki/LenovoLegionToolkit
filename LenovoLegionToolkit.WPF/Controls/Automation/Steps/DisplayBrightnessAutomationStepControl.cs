using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common;
using TextBox = Wpf.Ui.Controls.TextBox;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class DisplayBrightnessAutomationStepControl : AbstractAutomationStepControl<DisplayBrightnessAutomationStep>
    {
        private readonly TextBox _brightness = new()
        {
            PlaceholderText = "Brightness value",
            Width = 150,
        };

        private readonly StackPanel _stackPanel = new();

        public DisplayBrightnessAutomationStepControl(DisplayBrightnessAutomationStep step) : base(step)
        {
            Icon = SymbolRegular.BrightnessHigh48;
            Title = "Display brightness";
            Subtitle = "Change display brightness of the built-in display.\n\nWARNING: This action will not run correctly,\nif internal display is off.";

            SizeChanged += DisplayBrightnessAutomationStepControl_SizeChanged;
        }

        private void DisplayBrightnessAutomationStepControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            var newWidth = e.NewSize.Width / 3;
            _brightness.Width = newWidth;
        }

        public override IAutomationStep CreateAutomationStep() => new DisplayBrightnessAutomationStep(_brightness.Text);

        protected override UIElement? GetCustomControl()
        {
            _brightness.TextChanged += (s, e) =>
            {
                if (_brightness.Text != AutomationStep.Brightness)
                    RaiseChanged();
            };

            _stackPanel.Children.Add(_brightness);

            return _stackPanel;
        }

        protected override void OnFinishedLoading() { }

        protected override Task RefreshAsync()
        {
            _brightness.Text = AutomationStep.Brightness;
            return Task.CompletedTask;
        }
    }
}
