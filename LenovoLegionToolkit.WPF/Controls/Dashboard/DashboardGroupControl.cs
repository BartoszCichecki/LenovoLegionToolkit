using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        var initializedTasks = new List<Task>();

        stackPanel.Children.Add(new TextBlock
        {
            Text = _dashboardGroup.GetName(),
            FontSize = 24,
            FontWeight = FontWeights.Medium,
            Margin = new(0, 16, 0, 24)
        });

        var controls = await Task.WhenAll(_dashboardGroup.Items.Select(i => i.GetControlAsync()));

        foreach (var control in controls)
            stackPanel.Children.Add(control);

        initializedTasks.AddRange(controls.Select(c => c.InitializedTask));

        Content = stackPanel;

        await Task.WhenAll(initializedTasks);

        _initializedTaskCompletionSource.TrySetResult();
    }
}
