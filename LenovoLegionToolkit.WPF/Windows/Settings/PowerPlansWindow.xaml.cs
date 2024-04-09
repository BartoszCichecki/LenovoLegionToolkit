using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    private static readonly object DefaultValue = Resource.PowerPlansWindow_DefaultPowerPlan;

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

        var powerPlans = _powerPlanController.GetPowerPlans(true, false).OrderBy(x => x.Name).ToArray();
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

    private void Refresh(Selector comboBox, PowerPlan[] powerPlans, PowerModeState powerModeState)
    {
        var settingsPowerPlanGuid = _settings.Store.PowerPlans.GetValueOrDefault(powerModeState);
        var selectedValue = powerPlans.FirstOrDefault(pp => pp.Guid == settingsPowerPlanGuid);

        comboBox.Items.Clear();
        comboBox.Items.Add(DefaultValue);
        comboBox.Items.AddRange(powerPlans);
        comboBox.SelectedValue = selectedValue.Equals(default(PowerPlan)) ? DefaultValue : selectedValue;
    }

    private async Task PowerPlanChangedAsync(object value, PowerModeState powerModeState)
    {
        if (value is PowerPlan powerPlan)
            _settings.Store.PowerPlans[powerModeState] = powerPlan.Guid;
        if (value is string)
            _settings.Store.PowerPlans.Remove(powerModeState);
        _settings.SynchronizeStore();

        await _powerModeFeature.EnsureCorrectPowerPlanIsSetAsync();
    }

    private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var state = _quietModeComboBox.SelectedValue;
        if (state is null)
            return;

        await PowerPlanChangedAsync(state, PowerModeState.Quiet);
    }

    private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var state = _balanceModeComboBox.SelectedValue;
        if (state is null)
            return;

        await PowerPlanChangedAsync(state, PowerModeState.Balance);
    }

    private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var state = _performanceModeComboBox.SelectedValue;
        if (state is null)
            return;

        await PowerPlanChangedAsync(state, PowerModeState.Performance);
    }

    private async void GodModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var state = _godModeComboBox.SelectedValue;
        if (state is null)
            return;

        await PowerPlanChangedAsync(state, PowerModeState.GodMode);
    }
}
