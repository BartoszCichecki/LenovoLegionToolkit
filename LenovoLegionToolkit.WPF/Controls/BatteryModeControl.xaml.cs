using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class BatteryModeControl
    {
        private readonly BatteryFeature _feature = Container.Resolve<BatteryFeature>();

        public BatteryModeControl()
        {
            InitializeComponent();
        }

        protected override void FinishedLoading()
        {
            _comboBox.Visibility = Visibility.Visible;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsRefreshing)
                return;

            if (!_comboBox.TryGetSelectedItem(out BatteryState state))
                return;

            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            var items = Enum.GetValues<BatteryState>();
            var selectedItem = await _feature.GetStateAsync();
            _comboBox.SetItems(items, selectedItem, v => v.DisplayName());
        }
    }
}
