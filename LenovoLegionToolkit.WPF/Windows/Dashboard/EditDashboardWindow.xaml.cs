using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Controls.Dashboard.Edit;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Settings;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class EditDashboardWindow
{
    private readonly DashboardSettings _dashboardSettings = IoCContainer.Resolve<DashboardSettings>();

    private readonly DashboardGroup[] _groups;

    public event EventHandler? Apply;

    public EditDashboardWindow()
    {
        _groups = _dashboardSettings.Store.Groups ?? DashboardGroup.DefaultGroups;

        InitializeComponent();

        IsVisibleChanged += EditDashboardWindow_IsVisibleChanged;
    }

    private async void EditDashboardWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;
        _infoBar.Visibility = Visibility.Hidden;
        _applyRevertStackPanel.Visibility = Visibility.Hidden;

        var loadingTask = Task.Delay(TimeSpan.FromMilliseconds(500));

        var groups = _groups;

        _groupsScrollViewer.ScrollToTop();
        _groupsStackPanel.Children.Clear();

        _sensorsSwitch.IsChecked = _dashboardSettings.Store.ShowSensors;

        foreach (var group in groups)
            _groupsStackPanel.Children.Add(CreateGroupControl(group));

        GroupsChanged();

        await loadingTask;

        _applyRevertStackPanel.Visibility = Visibility.Visible;
        _infoBar.Visibility = Visibility.Visible;
        _loader.IsLoading = false;
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await MessageBoxHelper.ShowInputAsync(this,
            Resource.EditDashboardWindow_CreateGroup_Title,
            Resource.EditDashboardWindow_CreateGroup_Message,
            primaryButton: Resource.OK,
            secondaryButton: Resource.Cancel);

        if (string.IsNullOrEmpty(result))
            return;

        _groupsStackPanel.Children.Add(CreateGroupControl(new(DashboardGroupType.Custom, result)));
    }

    private void DefaultButton_Click(object sender, RoutedEventArgs e)
    {
        _dashboardSettings.Store.ShowSensors = true;
        _dashboardSettings.Store.Groups = null;
        _dashboardSettings.SynchronizeStore();

        Close();

        Apply?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        _dashboardSettings.Store.ShowSensors = _sensorsSwitch.IsChecked ?? true;
        _dashboardSettings.Store.Groups = _groupsStackPanel.Children
            .OfType<EditDashboardGroupControl>()
            .Select(c => c.GetDashboardGroup())
            .ToArray();
        _dashboardSettings.SynchronizeStore();

        Close();

        Apply?.Invoke(this, EventArgs.Empty);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private IEnumerable<DashboardItem> GetAllItems() =>
        _groupsStackPanel.Children
            .OfType<EditDashboardGroupControl>()
            .SelectMany(c => c.GetItems());

    private EditDashboardGroupControl CreateGroupControl(DashboardGroup dashboardGroup)
    {
        var control = new EditDashboardGroupControl(dashboardGroup, GetAllItems);
        control.MoveUp += (_, _) => MoveGroupUp(control);
        control.MoveDown += (_, _) => MoveGroupDown(control);
        control.Delete += (_, _) => DeleteGroup(control);
        control.Changed += (_, _) => GroupsChanged();
        return control;
    }

    private void GroupsChanged()
    {
        _groupsStackPanel.Children.OfType<EditDashboardGroupControl>().ForEach(c => c.RefreshAdd());
    }

    private void MoveGroupUp(UIElement control)
    {
        var index = _groupsStackPanel.Children.IndexOf(control);
        index--;

        if (index < 0)
            return;

        _groupsStackPanel.Children.Remove(control);
        _groupsStackPanel.Children.Insert(index, control);
    }

    private void MoveGroupDown(UIElement control)
    {
        var index = _groupsStackPanel.Children.IndexOf(control);
        index++;

        if (index >= _groupsStackPanel.Children.Count)
            return;

        _groupsStackPanel.Children.Remove(control);
        _groupsStackPanel.Children.Insert(index, control);
    }

    private void DeleteGroup(UIElement control)
    {
        _groupsStackPanel.Children.Remove(control);
    }
}
