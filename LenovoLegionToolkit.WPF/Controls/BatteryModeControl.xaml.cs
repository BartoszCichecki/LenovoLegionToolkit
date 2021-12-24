using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class BatteryModeControl
    {
        private readonly BatteryFeature _feature = Container.Resolve<BatteryFeature>();

        public BatteryModeControl()
        {
            InitializeComponent();
        }

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_comboBox.TryGetSelectedItem(out BatteryState state))
                return;

            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                var items = Enum.GetValues<BatteryState>();
                var selectedItem = await _feature.GetStateAsync();
                _comboBox.SetItems(items, selectedItem, v => v.DisplayName());
                Visibility = Visibility.Visible;
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
