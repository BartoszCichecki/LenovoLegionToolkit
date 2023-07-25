using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class BalanceModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly AIModeController _aiModeController = IoCContainer.Resolve<AIModeController>();
    private readonly AIChipController _aiChipController = IoCContainer.Resolve<AIChipController>();

    public BalanceModeSettingsWindow()
    {
        InitializeComponent();

        IsVisibleChanged += BalanceModeSettingsWindow_IsVisibleChanged;
    }

    private async void BalanceModeSettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
            return;

        _aiModeCheckBox.IsChecked = _aiModeController.IsEnabled;

        var mi = await Compatibility.GetMachineInformationAsync();
        if (mi.Features.AIChip)
        {
            _aiChipCheckBox.Visibility = Visibility.Visible;
            _aiChipCheckBox.IsEnabled = false;
        }
        else
        {
            _aiChipCheckBox.Visibility = Visibility.Collapsed;
            _aiChipCheckBox.IsEnabled = _aiModeController.IsEnabled;
        }
    }

    private void AiModeCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (!_aiChipCheckBox.IsVisible)
            return;

        _aiChipCheckBox.IsEnabled = _aiModeCheckBox.IsChecked ?? false;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var isAiModeChecked = _aiModeCheckBox.IsChecked ?? false;
        var isAiChipChecked = _aiChipCheckBox.IsChecked ?? false;

        _aiModeController.IsEnabled = isAiModeChecked;
        _aiChipController.IsEnabled = _aiChipCheckBox.IsVisible && isAiModeChecked && isAiChipChecked;

        await _powerModeFeature.SetStateAsync(PowerModeState.Balance);

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
