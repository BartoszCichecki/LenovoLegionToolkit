using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class WindowsPowerPlansWindow
{
    private static readonly WindowsPowerPlan DefaultValue = new(Guid.Empty, Resource.WindowsPowerPlansWindow_DefaultPowerPlan, false);

    private readonly WindowsPowerPlanController _windowsPowerPlanController = IoCContainer.Resolve<WindowsPowerPlanController>();
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private bool IsRefreshing => _loader.IsLoading;

    public WindowsPowerPlansWindow()
    {
        InitializeComponent();

        IsVisibleChanged += PowerPlansWindow_IsVisibleChanged;
    }

    private async void PowerPlansWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;

        var loadingTask = Task.Delay(500);

        var compatibility = await Compatibility.GetMachineInformationAsync();
        _aoAcWarningCard.Visibility = compatibility.Properties.SupportsAlwaysOnAc.status
            ? Visibility.Visible
            : Visibility.Collapsed;

        var powerPlans = _windowsPowerPlanController.GetPowerPlans().OrderBy(x => x.Name).Prepend(DefaultValue).ToArray();
        Refresh(_quietModeComboBox, powerPlans, PowerModeState.Quiet);
        Refresh(_balanceModeComboBox, powerPlans, PowerModeState.Balance);
        Refresh(_performanceModeComboBox, powerPlans, PowerModeState.Performance);

        var allStates = await _powerModeFeature.GetAllStatesAsync();
        if (allStates.Contains(PowerModeState.GodMode))
            Refresh(_godModeComboBox, powerPlans, PowerModeState.GodMode);
        else
            _godModeCardControl.Visibility = Visibility.Collapsed;

        await loadingTask;

        _loader.IsLoading = false;
    }

    private void Refresh(ComboBox comboBox, WindowsPowerPlan[] windowsPowerPlans, PowerModeState powerModeState)
    {
        var settingsPowerPlanGuid = _settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
        var selectedValue = windowsPowerPlans.FirstOrDefault(pp => pp.Guid == settingsPowerPlanGuid);
        comboBox.SetItems(windowsPowerPlans, selectedValue, pp => pp.Name);
    }

    private async Task WindowsPowerPlanChangedAsync(WindowsPowerPlan windowsPowerPlan, PowerModeState powerModeState)
    {
        if (IsRefreshing)
            return;

        _settings.Store.PowerPlans[powerModeState] = windowsPowerPlan.Guid;
        _settings.SynchronizeStore();

        await _powerModeFeature.EnsureCorrectWindowsPowerSettingsAreSetAsync();
    }

    private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_quietModeComboBox.TryGetSelectedItem(out WindowsPowerPlan windowsPowerPlan))
            await WindowsPowerPlanChangedAsync(windowsPowerPlan, PowerModeState.Quiet);
    }

    private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_balanceModeComboBox.TryGetSelectedItem(out WindowsPowerPlan windowsPowerPlan))
            await WindowsPowerPlanChangedAsync(windowsPowerPlan, PowerModeState.Balance);
    }

    private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_performanceModeComboBox.TryGetSelectedItem(out WindowsPowerPlan windowsPowerPlan))
            await WindowsPowerPlanChangedAsync(windowsPowerPlan, PowerModeState.Performance);
    }

    private async void GodModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_godModeComboBox.TryGetSelectedItem(out WindowsPowerPlan windowsPowerPlan))
            await WindowsPowerPlanChangedAsync(windowsPowerPlan, PowerModeState.GodMode);
    }
}
