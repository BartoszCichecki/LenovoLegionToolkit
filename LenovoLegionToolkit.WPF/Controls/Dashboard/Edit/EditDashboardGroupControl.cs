﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Dashboard;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;
using CardExpander = LenovoLegionToolkit.WPF.Controls.Custom.CardExpander;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard.Edit;

public class EditDashboardGroupControl : UserControl
{
    private readonly CardExpander _cardExpander = new()
    {
        Margin = new(0, 0, 0, 8)
    };

    private readonly CardHeaderControl _cardHeaderControl = new();

    private readonly StackPanel _stackPanel = new();

    private readonly StackPanel _itemsStackPanel = new();

    private readonly StackPanel _buttonsStackPanel = new()
    {
        Margin = new(0, 0, 16, 0),
        Orientation = Orientation.Horizontal
    };

    private readonly Button _editButton = new()
    {
        Icon = SymbolRegular.Edit24,
        ToolTip = Resource.Edit,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    private readonly Button _moveUpButton = new()
    {
        Icon = SymbolRegular.ArrowUp24,
        ToolTip = Resource.MoveUp,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    private readonly Button _moveDownButton = new()
    {
        Icon = SymbolRegular.ArrowDown24,
        ToolTip = Resource.MoveDown,
        MinWidth = 34,
        Height = 34,
        Margin = new(8, 0, 0, 0),
    };

    private readonly Button _deleteButton = new()
    {
        Icon = SymbolRegular.Dismiss24,
        ToolTip = Resource.Delete,
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
    public event EventHandler? Changed;

    private DashboardGroupType _dashboardGroupType;
    private readonly Func<IEnumerable<DashboardItem>> _getExistingItems;

    public EditDashboardGroupControl(DashboardGroup dashboardGroup, Func<IEnumerable<DashboardItem>> getExistingItems)
    {
        _dashboardGroupType = dashboardGroup.Type;

        _getExistingItems = getExistingItems;

        _editButton.Click += async (_, _) => await EditNameAsync();
        _moveUpButton.Click += (_, _) => MoveUp?.Invoke(this, EventArgs.Empty);
        _moveDownButton.Click += (_, _) => MoveDown?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => Delete?.Invoke(this, EventArgs.Empty);
        _addItemButton.Click += (_, _) => ShowAddItemWindow();

        _buttonsStackPanel.Children.Add(_editButton);
        _buttonsStackPanel.Children.Add(_moveUpButton);
        _buttonsStackPanel.Children.Add(_moveDownButton);
        _buttonsStackPanel.Children.Add(_deleteButton);

        foreach (var item in dashboardGroup.Items)
            _itemsStackPanel.Children.Add(CreateGroupControl(item));

        _stackPanel.Children.Add(_itemsStackPanel);
        _stackPanel.Children.Add(_addItemButton);

        _cardHeaderControl.Title = dashboardGroup.GetName();
        _cardHeaderControl.Accessory = _buttonsStackPanel;
        _cardExpander.Header = _cardHeaderControl;
        _cardExpander.Content = _stackPanel;

        AutomationProperties.SetName(_cardExpander, _cardHeaderControl.Title);
        AutomationProperties.SetName(_editButton, _cardHeaderControl.Title);
        AutomationProperties.SetName(_moveUpButton, _cardHeaderControl.Title);
        AutomationProperties.SetName(_moveDownButton, _cardHeaderControl.Title);
        AutomationProperties.SetName(_deleteButton, _cardHeaderControl.Title);

        Content = _cardExpander;
    }

    public DashboardGroup GetDashboardGroup()
    {
        var items = _itemsStackPanel.Children
            .OfType<EditDashboardItemControl>()
            .Select(c => c.DashboardItem)
            .ToArray();
        return new(_dashboardGroupType, _cardHeaderControl.Title, items);
    }

    public IEnumerable<DashboardItem> GetItems() =>
        _itemsStackPanel.Children
            .OfType<EditDashboardItemControl>()
            .Select(c => c.DashboardItem);

    private async Task EditNameAsync()
    {
        var text = _dashboardGroupType == DashboardGroupType.Custom ? _cardHeaderControl.Title : null;

        var result = await MessageBoxHelper.ShowInputAsync(this,
            Resource.EditDashboardGroupControl_EditGroup_Title,
            Resource.EditDashboardGroupControl_EditGroup_Message,
            text,
            primaryButton: Resource.OK,
            secondaryButton: Resource.Cancel);

        if (string.IsNullOrEmpty(result))
            return;

        _dashboardGroupType = DashboardGroupType.Custom;
        _cardHeaderControl.Title = result;
    }

    private void ShowAddItemWindow()
    {
        var window = new AddDashboardItemWindow(_getExistingItems, AddItem) { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void AddItem(DashboardItem dashboardItem)
    {
        _itemsStackPanel.Children.Add(CreateGroupControl(dashboardItem));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private EditDashboardItemControl CreateGroupControl(DashboardItem dashboardItem)
    {
        var control = new EditDashboardItemControl(dashboardItem);
        control.MoveUp += (_, _) => MoveItemUp(control);
        control.MoveDown += (_, _) => MoveItemDown(control);
        control.Delete += (_, _) => DeleteItem(control);
        return control;
    }

    private void MoveItemUp(UIElement control)
    {
        var index = _itemsStackPanel.Children.IndexOf(control);
        index--;

        if (index < 0)
            return;

        _itemsStackPanel.Children.Remove(control);
        _itemsStackPanel.Children.Insert(index, control);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void MoveItemDown(UIElement control)
    {
        var index = _itemsStackPanel.Children.IndexOf(control);
        index++;

        if (index >= _itemsStackPanel.Children.Count)
            return;

        _itemsStackPanel.Children.Remove(control);
        _itemsStackPanel.Children.Insert(index, control);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void DeleteItem(UIElement control)
    {
        _itemsStackPanel.Children.Remove(control);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void RefreshAdd() => _addItemButton.IsEnabled = Enum.GetValues<DashboardItem>().Except(_getExistingItems()).Any();
}
