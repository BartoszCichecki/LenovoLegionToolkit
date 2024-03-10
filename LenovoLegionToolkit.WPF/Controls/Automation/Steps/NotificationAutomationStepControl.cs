using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class NotificationAutomationStepControl : AbstractAutomationStepControl<NotificationAutomationStep>
{
    private readonly TextBox _scriptPath = new()
    {
        PlaceholderText = Resource.NotificationAutomationStepControl_NotificationText,
        Width = 300
    };

    private readonly StackPanel _stackPanel = new();

    public NotificationAutomationStepControl(NotificationAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.Rocket24;
        Title = Resource.NotificationAutomationStepControl_Title;

        SizeChanged += RunAutomationStepControl_SizeChanged;
    }

    private void RunAutomationStepControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
            return;

        var newWidth = e.NewSize.Width / 3;
        _scriptPath.Width = newWidth;
    }

    public override IAutomationStep CreateAutomationStep() => new NotificationAutomationStep(_scriptPath.Text);

    protected override UIElement GetCustomControl()
    {
        _scriptPath.TextChanged += (_, _) =>
        {
            if (_scriptPath.Text != AutomationStep.Text)
                RaiseChanged();
        };

        _stackPanel.Children.Add(_scriptPath);

        return _stackPanel;
    }

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync()
    {
        _scriptPath.Text = AutomationStep.Text ?? string.Empty;
        return Task.CompletedTask;
    }
}
