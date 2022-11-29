﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class RunAutomationStepControl : AbstractAutomationStepControl<RunAutomationStep>
{
    private readonly TextBox _scriptPath = new()
    {
        PlaceholderText = Resource.RunAutomationStepControl_ExePath,
        Width = 300,
        Margin = new(0, 0, 0, 8),
    };

    private readonly TextBox _scriptArguments = new()
    {
        PlaceholderText = Resource.RunAutomationStepControl_ExeArguments,
        Width = 300,
    };

    private readonly StackPanel _stackPanel = new();

    public RunAutomationStepControl(RunAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.WindowConsole20;
        Title = Resource.RunAutomationStepControl_Title;
        Subtitle = Resource.RunAutomationStepControl_Message;

        SizeChanged += RunAutomationStepControl_SizeChanged;
    }

    private void RunAutomationStepControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
            return;

        var newWidth = e.NewSize.Width / 3;
        _scriptPath.Width = newWidth;
        _scriptArguments.Width = newWidth;
    }

    public override IAutomationStep CreateAutomationStep() => new RunAutomationStep(_scriptPath.Text, _scriptArguments.Text);

    protected override UIElement? GetCustomControl()
    {
        _scriptPath.TextChanged += (s, e) =>
        {
            if (_scriptPath.Text != AutomationStep.ScriptPath)
                RaiseChanged();
        };
        _scriptArguments.TextChanged += (s, e) =>
        {
            if (_scriptArguments.Text != AutomationStep.ScriptPath)
                RaiseChanged();
        };

        _stackPanel.Children.Add(_scriptPath);
        _stackPanel.Children.Add(_scriptArguments);

        return _stackPanel;
    }

    protected override void OnFinishedLoading() { }

    protected override Task RefreshAsync()
    {
        _scriptPath.Text = AutomationStep.ScriptPath;
        _scriptArguments.Text = AutomationStep.ScriptArguments;
        return Task.CompletedTask;
    }
}