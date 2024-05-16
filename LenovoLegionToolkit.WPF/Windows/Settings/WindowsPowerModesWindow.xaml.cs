using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class WindowsPowerModesWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private bool IsRefreshing => _loader.IsLoading;

    public WindowsPowerModesWindow()
    {
        InitializeComponent();

        IsVisibleChanged += PowerModesWindow_IsVisibleChanged;
    }

    private async void PowerModesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;

        var loadingTask = Task.Delay(500);

        var powerModes = Enum.GetValues<WindowsPowerMode>();
        Refresh(_quietModeComboBox, powerModes, PowerModeState.Quiet);
        Refresh(_balanceModeComboBox, powerModes, PowerModeState.Balance);
        Refresh(_performanceModeComboBox, powerModes, PowerModeState.Performance);

        var allStates = await _powerModeFeature.GetAllStatesAsync();
        if (allStates.Contains(PowerModeState.GodMode))
            Refresh(_godModeComboBox, powerModes, PowerModeState.GodMode);
        else
            _godModeCardControl.Visibility = Visibility.Collapsed;

        await loadingTask;

        _loader.IsLoading = false;
    }

    private void Refresh(ComboBox comboBox, WindowsPowerMode[] windowsPowerPlans, PowerModeState powerModeState)
    {
        var selectedValue = _settings.Store.PowerModes.GetValueOrDefault(powerModeState, WindowsPowerMode.Balanced);
        comboBox.SetItems(windowsPowerPlans, selectedValue, pm => pm.GetDisplayName());
    }

    private async Task WindowsPowerModeChangedAsync(WindowsPowerMode windowsPowerMode, PowerModeState powerModeState)
    {
        if (IsRefreshing)
            return;

        _settings.Store.PowerModes[powerModeState] = windowsPowerMode;
        _settings.SynchronizeStore();

        await _powerModeFeature.EnsureCorrectWindowsPowerSettingsAreSetAsync();
    }

    private async void QuietModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_quietModeComboBox.TryGetSelectedItem(out WindowsPowerMode windowsPowerMode))
            await WindowsPowerModeChangedAsync(windowsPowerMode, PowerModeState.Quiet);
    }

    private async void BalanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_balanceModeComboBox.TryGetSelectedItem(out WindowsPowerMode windowsPowerMode))
            await WindowsPowerModeChangedAsync(windowsPowerMode, PowerModeState.Balance);
    }

    private async void PerformanceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_performanceModeComboBox.TryGetSelectedItem(out WindowsPowerMode windowsPowerMode))
            await WindowsPowerModeChangedAsync(windowsPowerMode, PowerModeState.Performance);
    }

    private async void GodModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_godModeComboBox.TryGetSelectedItem(out WindowsPowerMode windowsPowerMode))
            await WindowsPowerModeChangedAsync(windowsPowerMode, PowerModeState.GodMode);
    }
}