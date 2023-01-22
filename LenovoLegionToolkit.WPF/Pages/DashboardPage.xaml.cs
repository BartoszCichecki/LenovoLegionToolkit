﻿using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Windows.Dashboard;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class DashboardPage
{
    public DashboardPage()
    {
        InitializeComponent();

        SizeChanged += DashboardPage_SizeChanged;
    }

    private void DashboardPage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
            return;

        if (e.NewSize.Width > 1000)
            Expand();
        else
            Collapse();
    }

    private void Expand()
    {
        _column1.Width = new(1, GridUnitType.Star);

        Grid.SetRow(_powerStackPanel, 0);
        Grid.SetColumn(_powerStackPanel, 0);

        Grid.SetRow(_graphicsStackPanel, 0);
        Grid.SetColumn(_graphicsStackPanel, 1);

        Grid.SetRow(_displayStackPanel, 1);
        Grid.SetColumn(_displayStackPanel, 0);

        Grid.SetRow(_otherStackPanel, 1);
        Grid.SetColumn(_otherStackPanel, 1);
    }

    private void Collapse()
    {
        _column1.Width = new(0, GridUnitType.Pixel);

        Grid.SetRow(_powerStackPanel, 0);
        Grid.SetColumn(_powerStackPanel, 0);

        Grid.SetRow(_graphicsStackPanel, 1);
        Grid.SetColumn(_graphicsStackPanel, 0);

        Grid.SetRow(_displayStackPanel, 2);
        Grid.SetColumn(_displayStackPanel, 0);

        Grid.SetRow(_otherStackPanel, 3);
        Grid.SetColumn(_otherStackPanel, 0);
    }

    private void EditDashboard_OnClick(object sender, RoutedEventArgs e)
    {
        var window = new EditDashboardWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }
}
