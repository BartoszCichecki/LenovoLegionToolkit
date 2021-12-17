using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                Refresh();
        }

        private void AutorunToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _autorunToggleButton.IsChecked;
            if (state.Value)
                Autorun.Enable();
            else
                Autorun.Disable();
        }
        private void VantageToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _vantageToggleButton.IsChecked;
            if (state.Value)
                Vantage.Enable();
            else
                Vantage.Disable();
        }

        private void Refresh()
        {
            _autorunToggleButton.IsChecked = Autorun.IsEnabled;

            var vantageStatus = Vantage.Status;
            _vantageCard.Visibility = vantageStatus != VantageStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
            _vantageToggleButton.IsChecked = vantageStatus == VantageStatus.Enabled;
        }
    }
}
