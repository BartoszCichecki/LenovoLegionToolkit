using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class BalanceModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly AIController _aiController = IoCContainer.Resolve<AIController>();

    public BalanceModeSettingsWindow()
    {
        InitializeComponent();

        IsVisibleChanged += BalanceModeSettingsWindow_IsVisibleChanged;
    }

    private void BalanceModeSettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
            return;

        _aiModeCheckBox.IsChecked = _aiController.IsAIModeEnabled;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var isAiModeChecked = _aiModeCheckBox.IsChecked ?? false;

        _aiController.IsAIModeEnabled = isAiModeChecked;

        await _aiController.StopAsync();
        await _powerModeFeature.SetStateAsync(PowerModeState.Balance);
        await _aiController.StartIfNeededAsync();

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
