using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class DashboardGroupControl : UserControl
{
    private readonly DashboardGroup _dashboardGroup;

    public DashboardGroupControl(DashboardGroup dashboardGroup)
    {
        _dashboardGroup = dashboardGroup;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var stackPanel = new StackPanel { Margin = new(0, 0, 16, 0) };

        stackPanel.Children.Add(new TextBlock
        {
            Text = _dashboardGroup.GetName(),
            FontSize = 24,
            FontWeight = FontWeights.Medium,
            Margin = new(0, 16, 0, 24)
        });

        foreach (var item in _dashboardGroup.Items)
            stackPanel.Children.Add(item.GetControl());

        Content = stackPanel;
    }
}
