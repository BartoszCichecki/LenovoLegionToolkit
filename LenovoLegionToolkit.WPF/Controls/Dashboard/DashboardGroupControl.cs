﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class DashboardGroupControl : UserControl
{
    private readonly TaskCompletionSource _initializedTaskCompletionSource = new();

    private readonly DashboardGroup _dashboardGroup;

    public Task InitializedTask => _initializedTaskCompletionSource.Task;

    public DashboardGroupControl(DashboardGroup dashboardGroup)
    {
        _dashboardGroup = dashboardGroup;

        Initialized += DashboardGroupControl_Initialized;
    }

    private async void DashboardGroupControl_Initialized(object? sender, System.EventArgs e)
    {
        var stackPanel = new StackPanel { Margin = new(0, 0, 16, 0) };

        var textBlock = new TextBlock
        {
            Text = _dashboardGroup.GetName(),
            Focusable = true,
            FontSize = 24,
            FontWeight = FontWeights.Medium,
            Margin = new(0, 16, 0, 24)
        };
        AutomationProperties.SetName(textBlock, textBlock.Text);
        stackPanel.Children.Add(textBlock);

        var controlsTasks = _dashboardGroup.Items.Select(i => i.GetControlAsync());
        var controls = await Task.WhenAll(controlsTasks);

        foreach (var control in controls.SelectMany(c => c))
        {
            stackPanel.Children.Add(control);
        }

        Content = stackPanel;

        _initializedTaskCompletionSource.TrySetResult();
    }
}
