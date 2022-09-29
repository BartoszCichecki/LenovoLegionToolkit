using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class BalanceModeSettingsWindow
    {
        private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
        private readonly AIModeController _aiModeController = IoCContainer.Resolve<AIModeController>();

        public BalanceModeSettingsWindow()
        {
            InitializeComponent();

            _aiModeCheckBox.IsChecked = _aiModeController.IsEnabled;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _aiModeController.IsEnabled = _aiModeCheckBox.IsChecked ?? false;

            await _powerModeFeature.SetStateAsync(PowerModeState.Balance);

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
