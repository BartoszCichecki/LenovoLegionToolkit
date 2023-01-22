using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard.Edit;

public class EditDashboardGroupControl : UserControl
{
    private readonly CardExpander _cardExpander = new()
    {
        Margin = new(0, 0, 0, 8),
        IsExpanded = true
    };

    private readonly CardHeaderControl _cardHeaderControl = new();

    private readonly StackPanel _stackPanel = new();

    private readonly StackPanel _itemsStackPanel = new();

    private readonly StackPanel _buttonsStackPanel = new()
    {
        Margin = new(0, 0, 16, 0),
        Orientation = Orientation.Horizontal
    };

    private readonly Button _moveUpButton = new()
    {
        Icon = SymbolRegular.ArrowUp24,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    private readonly Button _moveDownButton = new()
    {
        Icon = SymbolRegular.ArrowDown24,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    private readonly Button _deleteButton = new()
    {
        Icon = SymbolRegular.Dismiss24,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    public event EventHandler? MoveUp;
    public event EventHandler? MoveDown;
    public event EventHandler? Delete;

    private readonly DashboardGroupType _dashboardGroupType;
    private readonly string _dashboardGroupName;
    private readonly DashboardItem[] _dashboardGroupItems;
    private readonly Func<IEnumerable<DashboardItem>> _getAllItems;

    public EditDashboardGroupControl(DashboardGroup dashboardGroup, Func<IEnumerable<DashboardItem>> getAllItems)
    {
        _dashboardGroupType = dashboardGroup.Type;
        _dashboardGroupName = dashboardGroup.GetName();
        _dashboardGroupItems = dashboardGroup.Items;
        _getAllItems = getAllItems;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        _moveUpButton.Click += (s, e) => MoveUp?.Invoke(this, EventArgs.Empty);
        _moveDownButton.Click += (s, e) => MoveDown?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (s, e) => Delete?.Invoke(this, EventArgs.Empty);

        _buttonsStackPanel.Children.Add(_moveUpButton);
        _buttonsStackPanel.Children.Add(_moveDownButton);
        _buttonsStackPanel.Children.Add(_deleteButton);

        foreach (var item in _dashboardGroupItems)
            _itemsStackPanel.Children.Add(CreateGroupControl(item));

        _stackPanel.Children.Add(_itemsStackPanel);

        _cardHeaderControl.Title = _dashboardGroupName;
        _cardHeaderControl.Accessory = _buttonsStackPanel;
        _cardExpander.Header = _cardHeaderControl;
        _cardExpander.Content = _stackPanel;

        Content = _cardExpander;
    }

    public DashboardGroup GetDashboardGroup()
    {
        var items = _itemsStackPanel.Children.OfType<EditDashboardItemControl>().Select(c => c.DashboardItem).ToArray();
        return new(_dashboardGroupType, _dashboardGroupName, items);
    }

    public IEnumerable<DashboardItem> GetItems()
    {
        return _itemsStackPanel.Children.OfType<EditDashboardItemControl>().Select(c => c.DashboardItem);
    }

    private Control CreateGroupControl(DashboardItem dashboardItem)
    {
        var control = new EditDashboardItemControl(dashboardItem);
        control.MoveUp += (s, e) => MoveItemUp(control);
        control.MoveDown += (s, e) => MoveItemDown(control);
        control.Delete += (s, e) => DeleteItem(control);
        return control;
    }

    private void MoveItemUp(Control control)
    {
        var index = _itemsStackPanel.Children.IndexOf(control);
        index--;

        if (index < 0)
            return;

        _itemsStackPanel.Children.Remove(control);
        _itemsStackPanel.Children.Insert(index, control);
    }

    private void MoveItemDown(Control control)
    {
        var index = _itemsStackPanel.Children.IndexOf(control);
        index++;

        if (index >= _itemsStackPanel.Children.Count)
            return;

        _itemsStackPanel.Children.Remove(control);
        _itemsStackPanel.Children.Insert(index, control);
    }

    private void DeleteItem(Control control)
    {
        _itemsStackPanel.Children.Remove(control);
    }
}