using System;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard.Edit;

public class EditDashboardItemControl : UserControl
{
    public DashboardItem DashboardItem { get; }

    private readonly CardControl _cardControl = new()
    {
        Margin = new(0, 0, 0, 8),
    };

    private readonly CardHeaderControl _cardHeaderControl = new();

    private readonly StackPanel _stackPanel = new()
    {
        Orientation = Orientation.Horizontal,
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

    public event EventHandler? MoveUp;
    public event EventHandler? MoveDown;
    public event EventHandler? Delete;

    public EditDashboardItemControl(DashboardItem dashboardItem)
    {
        DashboardItem = dashboardItem;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        _moveUpButton.Click += (_, _) => MoveUp?.Invoke(this, EventArgs.Empty);
        _moveDownButton.Click += (_, _) => MoveDown?.Invoke(this, EventArgs.Empty);
        _deleteButton.Click += (_, _) => Delete?.Invoke(this, EventArgs.Empty);

        _stackPanel.Children.Add(_moveUpButton);
        _stackPanel.Children.Add(_moveDownButton);
        _stackPanel.Children.Add(_deleteButton);

        _cardHeaderControl.Title = DashboardItem.GetTitle();
        _cardHeaderControl.Accessory = _stackPanel;
        _cardControl.Icon = DashboardItem.GetIcon();
        _cardControl.Header = _cardHeaderControl;

        Content = _cardControl;
    }
}
