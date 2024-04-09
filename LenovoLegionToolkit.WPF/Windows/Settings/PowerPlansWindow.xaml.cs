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

public partial class PowerPlansWindow
{
    private static readonly PowerPlan DefaultValue = new(Guid.Empty, Resource.PowerPlansWindow_DefaultPowerPlan, false, false);

    private readonly PowerPlanController _powerPlanController = IoCContainer.Resolve<PowerPlanController>();
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    public PowerPlansWindow()
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

        var powerPlans = _powerPlanController.GetPowerPlans(true, false).OrderBy(x => x.Name).Prepend(DefaultValue).ToArray();
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

    private void Refresh(ComboBox comboBox, PowerPlan[] powerPlans, PowerModeState powerModeState)
    {
        var settingsPowerPlanGuid = _settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
        var selectedValue = powerPlans.FirstOrDefault(pp => pp.Guid == settingsPowerPlanGuid);
        comboBox.SetItems(powerPlans, selectedValue, pp => pp.Name);
    }

    private async Task PowerPlanChangedAsync(PowerPlan powerPlan, PowerModeState powerModeState)
    {
        _settings.Store.PowerPlans[powerModeState] = powerPlan.Guid;
        _settings.SynchronizeStore();

        await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync();
    }

    private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_quietModeComboBox.TryGetSelectedItem(out PowerPlan powerPlan))
            await PowerPlanChangedAsync(powerPlan, PowerModeState.Quiet);
    }

    private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_balanceModeComboBox.TryGetSelectedItem(out PowerPlan powerPlan))
            await PowerPlanChangedAsync(powerPlan, PowerModeState.Balance);
    }

    private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_performanceModeComboBox.TryGetSelectedItem(out PowerPlan powerPlan))
            await PowerPlanChangedAsync(powerPlan, PowerModeState.Performance);
    }

    private async void GodModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_godModeComboBox.TryGetSelectedItem(out PowerPlan powerPlan))
            await PowerPlanChangedAsync(powerPlan, PowerModeState.GodMode);
    }
}
