using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Controls.Dashboard.Edit;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Settings;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class EditDashboardWindow
{
    private readonly DashboardSettings _dashboardSettings = IoCContainer.Resolve<DashboardSettings>();

    private DashboardGroup[] _groups;

    public event EventHandler? Apply;

    public EditDashboardWindow()
    {
        _groups = _dashboardSettings.Store.Groups;

        InitializeComponent();
    }

    private async void EditDashboardWindow_Loaded(object _1, RoutedEventArgs _2) => await RefreshAsync();

    private async void EditDashboardWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;
        _infoBar.Visibility = Visibility.Hidden;
        _applyRevertStackPanel.Visibility = Visibility.Hidden;

        var loadingTask = Task.Delay(500);

        var groups = _groups;

        _groupsScrollViewer.ScrollToTop();
        _groupsStackPanel.Children.Clear();

        foreach (var group in groups)
            _groupsStackPanel.Children.Add(CreateGroupControl(group));

        await loadingTask;

        _applyRevertStackPanel.Visibility = Visibility.Visible;
        _infoBar.Visibility = Visibility.Visible;
        _loader.IsLoading = false;
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await MessageBoxHelper.ShowInputAsync(this, "Create group", "Name", primaryButton: Resource.OK, secondaryButton: Resource.Cancel);

        if (string.IsNullOrEmpty(result))
            return;

        _groupsStackPanel.Children.Add(CreateGroupControl(new(DashboardGroupType.Custom, result)));
    }

    private async void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _groups = DashboardGroup.DefaultGroups;

        await RefreshAsync();
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        var groups = _groupsStackPanel.Children
            .OfType<EditDashboardGroupControl>()
            .Select(c => c.GetDashboardGroup())
            .ToArray();

        _dashboardSettings.Store.Groups = groups;
        _dashboardSettings.SynchronizeStore();

        Close();

        Apply?.Invoke(this, EventArgs.Empty);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void InfoBarDismissButton_Click(object sender, RoutedEventArgs e)
    {
        _infoBar.Visibility = Visibility.Collapsed;
    }

    private IEnumerable<DashboardItem> GetAllItems() =>
        _groupsStackPanel.Children
            .OfType<EditDashboardGroupControl>()
            .SelectMany(c => c.GetItems());

    private Control CreateGroupControl(DashboardGroup dashboardGroup)
    {
        var control = new EditDashboardGroupControl(dashboardGroup, GetAllItems);
        control.MoveUp += (s, e) => MoveGroupUp(control);
        control.MoveDown += (s, e) => MoveGroupDown(control);
        control.Delete += (s, e) => DeleteGroup(control);
        return control;
    }

    private void MoveGroupUp(Control control)
    {
        var index = _groupsStackPanel.Children.IndexOf(control);
        index--;

        if (index < 0)
            return;

        _groupsStackPanel.Children.Remove(control);
        _groupsStackPanel.Children.Insert(index, control);
    }

    private void MoveGroupDown(Control control)
    {
        var index = _groupsStackPanel.Children.IndexOf(control);
        index++;

        if (index >= _groupsStackPanel.Children.Count)
            return;

        _groupsStackPanel.Children.Remove(control);
        _groupsStackPanel.Children.Insert(index, control);
    }

    private void DeleteGroup(Control control)
    {
        _groupsStackPanel.Children.Remove(control);
    }
}
