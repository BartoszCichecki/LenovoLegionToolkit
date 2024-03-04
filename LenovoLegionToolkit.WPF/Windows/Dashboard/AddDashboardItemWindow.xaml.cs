using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using CardControl = LenovoLegionToolkit.WPF.Controls.Custom.CardControl;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class AddDashboardItemWindow
{
    private readonly Func<IEnumerable<DashboardItem>> _existingItems;
    private readonly Action<DashboardItem> _addDashboardItem;

    public AddDashboardItemWindow(Func<IEnumerable<DashboardItem>> existingItems, Action<DashboardItem> addDashboardItem)
    {
        _existingItems = existingItems;
        _addDashboardItem = addDashboardItem;

        InitializeComponent();

        IsVisibleChanged += AddAutomationStepWindow_IsVisibleChanged;
    }

    private async void AddAutomationStepWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private Task RefreshAsync()
    {
        _content.Children.Clear();

        var allItems = Enum.GetValues<DashboardItem>();
        var existingItems = _existingItems().ToArray();

        foreach (var item in allItems.Except(existingItems))
            _content.Children.Add(CreateCardControl(item));

        return Task.CompletedTask;
    }

    private CardControl CreateCardControl(DashboardItem item)
    {
        var control = new CardControl
        {
            Icon = item.GetIcon(),
            Header = new CardHeaderControl
            {
                Title = item.GetTitle(),
                Accessory = new SymbolIcon { Symbol = SymbolRegular.ChevronRight24 }
            },
            Margin = new(0, 8, 0, 0),
        };

        control.Click += (_, _) =>
        {
            _addDashboardItem(item);
            Close();
        };

        return control;
    }
}
