using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = Wpf.Ui.Controls.MenuItem;

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

    private readonly Button _addItemButton = new()
    {
        MinWidth = 120,
        HorizontalAlignment = HorizontalAlignment.Right,
        Appearance = ControlAppearance.Primary,
        Content = Resource.Add,
        Margin = new(0, 8, 0, 0),
    };

    public event EventHandler? MoveUp;
    public event EventHandler? MoveDown;
    public event EventHandler? Delete;

    private readonly DashboardGroupType _dashboardGroupType;
    private readonly string _dashboardGroupName;
    private readonly DashboardItem[] _dashboardGroupItems;
    private readonly Func<IEnumerable<DashboardItem>> _getExistingItems;

    public EditDashboardGroupControl(DashboardGroup dashboardGroup, Func<IEnumerable<DashboardItem>> getExistingItems)
    {
        _dashboardGroupType = dashboardGroup.Type;
        _dashboardGroupName = dashboardGroup.GetName();
        _dashboardGroupItems = dashboardGroup.Items;
        _getExistingItems = getExistingItems;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        _moveUpButton.Click += (_, _) => MoveUp?.Invoke(this, EventArgs.Empty);
        _moveDownButton.Click += (_, _) => MoveDown?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => Delete?.Invoke(this, EventArgs.Empty);
        _addItemButton.Click += (_, _) => ShowContextMenu();

        _buttonsStackPanel.Children.Add(_moveUpButton);
        _buttonsStackPanel.Children.Add(_moveDownButton);
        _buttonsStackPanel.Children.Add(_deleteButton);

        foreach (var item in _dashboardGroupItems)
            _itemsStackPanel.Children.Add(CreateGroupControl(item));

        _stackPanel.Children.Add(_itemsStackPanel);
        _stackPanel.Children.Add(_addItemButton);

        _cardHeaderControl.Title = _dashboardGroupName;
        _cardHeaderControl.Accessory = _buttonsStackPanel;
        _cardExpander.Header = _cardHeaderControl;
        _cardExpander.Content = _stackPanel;

        Content = _cardExpander;
    }

    public DashboardGroup GetDashboardGroup()
    {
        var items = _itemsStackPanel.Children
            .OfType<EditDashboardItemControl>()
            .Select(c => c.DashboardItem)
            .ToArray();
        return new(_dashboardGroupType, _dashboardGroupName, items);
    }

    public IEnumerable<DashboardItem> GetItems() =>
        _itemsStackPanel.Children
            .OfType<EditDashboardItemControl>()
            .Select(c => c.DashboardItem);

    private void ShowContextMenu()
    {
        var allItems = Enum.GetValues<DashboardItem>();
        var existingItems = _getExistingItems().ToArray();

        var menuItems = new List<MenuItem>();

        foreach (var item in allItems)
        {
            var menuItem = new MenuItem { SymbolIcon = item.GetIcon(), Header = item.GetTitle() };
            menuItem.Click += (_, _) => AddItem(item);
            menuItem.IsEnabled = !existingItems.Contains(item);
            menuItems.Add(menuItem);
        }

        var contextMenu = new ContextMenu
        {
            PlacementTarget = _addItemButton,
            Placement = PlacementMode.Bottom,
        };

        foreach (var menuItem in menuItems.OrderBy(mi => mi.Header))
            contextMenu.Items.Add(menuItem);

        _addItemButton.ContextMenu = contextMenu;
        _addItemButton.ContextMenu.IsOpen = true;
    }

    private void AddItem(DashboardItem dashboardItem)
    {
        _itemsStackPanel.Children.Add(CreateGroupControl(dashboardItem));
    }

    private Control CreateGroupControl(DashboardItem dashboardItem)
    {
        var control = new EditDashboardItemControl(dashboardItem);
        control.MoveUp += (_, _) => MoveItemUp(control);
        control.MoveDown += (_, _) => MoveItemDown(control);
        control.Delete += (_, _) => DeleteItem(control);
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
