using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Microsoft.Win32;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class SpectrumKeyboardBacklightImportProfileAutomationStepControl : AbstractAutomationStepControl<SpectrumKeyboardBacklightImportProfileAutomationStep>
{
    private readonly TextBox _path = new()
    {
        PlaceholderText = Resource.SpectrumKeyboardBacklightImportProfileAutomationStepControl_Path,
        Width = 300
    };

    private readonly Button _openButton = new()
    {
        Icon = SymbolRegular.MoreHorizontal24,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0)
    };

    private readonly StackPanel _stackPanel = new()
    {
        Orientation = Orientation.Horizontal
    };

    public SpectrumKeyboardBacklightImportProfileAutomationStepControl(SpectrumKeyboardBacklightImportProfileAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.BrightnessHigh24;
        Title = Resource.SpectrumKeyboardBacklightImportProfileAutomationStepControl_Title;
        Subtitle = Resource.SpectrumKeyboardBacklightImportProfileAutomationStepControl_Message;

        SizeChanged += RunAutomationStepControl_SizeChanged;
    }

    private void RunAutomationStepControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
            return;

        var newWidth = e.NewSize.Width / 3;
        _path.Width = newWidth;
    }

    public override IAutomationStep CreateAutomationStep() => new SpectrumKeyboardBacklightImportProfileAutomationStep(_path.Text);

    protected override UIElement GetCustomControl()
    {
        _path.TextChanged += (_, _) =>
        {
            if (_path.Text != AutomationStep.Path)
                RaiseChanged();
        };

        _openButton.Click += (_, _) =>
        {
            var ofd = new OpenFileDialog
            {
                Title = Resource.Import,
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                Filter = "Json Files (.json)|*.json",
                CheckFileExists = true,
            };
            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            _path.Text = ofd.FileName;
        };

        _stackPanel.Children.Add(_path);
        _stackPanel.Children.Add(_openButton);

        return _stackPanel;
    }

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync()
    {
        _path.Text = AutomationStep.Path;
        return Task.CompletedTask;
    }
}
